using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IStateMachineCodeGenerator;
using System.Text.RegularExpressions;


namespace StateMachineCodeGeneratorSystem
{
    public class CFileUpdater : IFileUpdater
    {
        private string m_strFileNameAndPath;
        private string[] m_astrPrimaryFileContents;
        private EFileUpdaterStatus m_eFileUpdaterStatus;
        private Dictionary<string, Tuple<int, int>> m_dictRegionNameAndFileLineNumbers = new Dictionary<string, Tuple<int, int>>();
        private Dictionary<string, Dictionary<string, Tuple<int, int, int>>> m_dictOfDictionariesOfMethodNamesAndFileLocations = new Dictionary<string, Dictionary<string, Tuple<int, int, int>>>();


        private enum ESearch { SearchingForStartOfMethod, SearchingForMethodName, SearchingForEndOfMethod };

        #region Properties

        public EFileUpdaterStatus Status
        {
            get { return m_eFileUpdaterStatus; }
        }

        #endregion Properties


        public EFileUpdaterStatus UpdateStatus()
        {
            FileInfo oFileInfo;

            try
            {
                if (File.Exists(m_strFileNameAndPath) == false)
                {
                    m_eFileUpdaterStatus = EFileUpdaterStatus.FileDoesntExist;
                }
                else
                {
                    oFileInfo = new FileInfo(m_strFileNameAndPath);

                    if (oFileInfo.IsReadOnly == true)
                    {
                        m_eFileUpdaterStatus = EFileUpdaterStatus.FileIsReadOnly;
                    }
                    else
                    {
                        m_eFileUpdaterStatus = EFileUpdaterStatus.OK;
                    }
                }
            }
            catch (Exception)
            {
                m_eFileUpdaterStatus = EFileUpdaterStatus.FileDoesntExist;
            }

            return m_eFileUpdaterStatus;
        }


        public EFileUpdaterStatus LoadFileIntoMemory()
        {
            if (File.Exists(m_strFileNameAndPath) == false)
            {
                m_astrPrimaryFileContents = null;
                m_eFileUpdaterStatus = EFileUpdaterStatus.FileDoesntExist;
            }
            else
            {

                m_astrPrimaryFileContents = File.ReadAllLines(m_strFileNameAndPath);
                File.WriteAllLines(m_strFileNameAndPath + ".bak", m_astrPrimaryFileContents); // Back up File. Maybe do somewhere else
                m_eFileUpdaterStatus = EFileUpdaterStatus.OK;
            }

            return m_eFileUpdaterStatus;
        }

        public EFileUpdaterStatus FindHeaderCommentExtentsInLoadedFile(out int nStartingIndex, out int nEndingIndex)
        {
            int nIndex;
            EFileUpdaterStatus eFileUpdaterStatus = EFileUpdaterStatus.Failed;
            nStartingIndex = -1;
            nEndingIndex = -1;

            if (m_astrPrimaryFileContents != null)
            {
                if (m_astrPrimaryFileContents[0].StartsWith(@"//") == true)
                {
                    nStartingIndex = 0;

                    for (nIndex = 1; nIndex < m_astrPrimaryFileContents.Count(); nIndex++)
                    {
                        if (m_astrPrimaryFileContents[nIndex].StartsWith(@"//") == false)
                        {
                            nEndingIndex = nIndex - 1;
                            break;
                        }
                    }

                    eFileUpdaterStatus = EFileUpdaterStatus.OK;
                }
            }

            return eFileUpdaterStatus;
        }



        public EFileUpdaterStatus FindRegionExtentsInLoadedFile(string strRegionName, out int nStartingIndex, out int nEndingIndex)
        {
            bool bLocatingStartOfRegion = true;
            int nIndex;
            int nRegionEndRegionCounter = 0;
            string strLineOfFile;
            string strRegionPlusRegionName = "#region " + strRegionName;
            nStartingIndex = -1;
            nEndingIndex = -1;
            Tuple<int, int> tupFileLocations;

            if (m_astrPrimaryFileContents == null)
            {
                return EFileUpdaterStatus.Failed;     
            }


            for (nIndex = 0; nIndex < m_astrPrimaryFileContents.Count(); nIndex++)
            {
                strLineOfFile = m_astrPrimaryFileContents[nIndex].Trim();


                if (bLocatingStartOfRegion == true)
                {
                    if (strLineOfFile == strRegionPlusRegionName)
                    {
                        nStartingIndex = nIndex;
                        bLocatingStartOfRegion = false;
                    }
                }
                else
                {
                    if (nRegionEndRegionCounter == 0)
                    {
                        if (strLineOfFile.StartsWith(@"#endregion") == true)
                        {
                            nEndingIndex = nIndex;
                            break;
                        }
                        else if (strLineOfFile.StartsWith(@"#region") == true)
                        {
                            nRegionEndRegionCounter++;
                        }
                    }
                    else // Looking for an additional End region which we are not interested in
                    {
                        if (strLineOfFile.StartsWith(@"#endregion") == true)
                        {
                            nRegionEndRegionCounter--;
                        }
                        else if (strLineOfFile.StartsWith(@"#region") == true)
                        {
                            nRegionEndRegionCounter++;
                        }
                    }
                }
            }

            if ((nStartingIndex != -1) && (nEndingIndex != -1))
            {
                if (m_dictRegionNameAndFileLineNumbers.TryGetValue(strRegionName, out tupFileLocations) == false)
                {
                    m_dictRegionNameAndFileLineNumbers.Add(strRegionName, new Tuple<int, int>(nStartingIndex, nEndingIndex));
                }
                return EFileUpdaterStatus.OK;
            }
            else
            {
                return EFileUpdaterStatus.Failed;
            }
        }

        public EFileUpdaterStatus LocateMethodsAndPropertiesInLoadedFile(string strRegionName)
        {
            bool bSearchingForStarSlash = false;
            int nIndex;
            int nFileLineIndex;
            int nStartingIndex;
            int nEndingIndex;
            int nStartOfMethodIndex = 0;
            int nStartOfMethodIncludingDocumentationIndex = 0;
            int nEndOfMethodIndex = 0;
            int nIndexOfSlashSlash;
            int nIndexOfDocumentation;
            int nIndexOfSlashStar;
            int nIndexOfStarSlash;
            int nOpenBraceCount = 0;

            string strMethodName = "";

            string strLineOfFile;

            string[] astrMethodLineElements = null;
            Dictionary<string, Tuple<int, int, int>> m_dictMethodNameAndFileLocation = new Dictionary<string, Tuple<int, int, int>>();

            ESearch eSearch = ESearch.SearchingForStartOfMethod;

            if (FindRegionExtentsInLoadedFile(strRegionName, out nStartingIndex, out nEndingIndex) == EFileUpdaterStatus.OK)
            {
                for (nFileLineIndex = nStartingIndex + 1; nFileLineIndex < nEndingIndex; nFileLineIndex++)
                {
                    strLineOfFile = m_astrPrimaryFileContents[nFileLineIndex].Trim();

                    nIndexOfSlashSlash = strLineOfFile.IndexOf(@"//");
                    nIndexOfDocumentation = strLineOfFile.IndexOf(@"///");
                    nIndexOfSlashStar = strLineOfFile.IndexOf(@"/*");
                    nIndexOfStarSlash = strLineOfFile.IndexOf(@"*/");

                    if (bSearchingForStarSlash == true)
                    {

                    }
                    else
                    {

                    }


                    switch (eSearch)
                    {
                        case ESearch.SearchingForStartOfMethod:
                            if (strLineOfFile != "")
                            {
                                nStartOfMethodIncludingDocumentationIndex = nFileLineIndex;
                                eSearch = ESearch.SearchingForMethodName;
                            }
                            break;


                        case ESearch.SearchingForMethodName:

                            // Check for Triple Slash or Double Slash
                            if (nIndexOfDocumentation == -1)
                            {
                                astrMethodLineElements = strLineOfFile.Split(new char[] { ' ', '(', '[' }, StringSplitOptions.None);

                                if ((astrMethodLineElements[0] == "public") || (astrMethodLineElements[0] == "private"))
                                {
                                    if ((astrMethodLineElements[1] == "void") || (astrMethodLineElements[1] == "bool") || (astrMethodLineElements[1] == "int"))
                                    {
                                        strMethodName = astrMethodLineElements[2];
                                        nStartOfMethodIndex = nFileLineIndex;
                                        eSearch = ESearch.SearchingForEndOfMethod;

                                        if (astrMethodLineElements.Count() > 3)
                                        {
                                            for (nIndex = 3; nIndex < astrMethodLineElements.Count(); nIndex++)
                                            {
                                                if (astrMethodLineElements[nIndex] == "{")
                                                {
                                                    nOpenBraceCount = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {

                                }
                            }


                            break;

                        case ESearch.SearchingForEndOfMethod:
                            for (nIndex = 0; nIndex < strLineOfFile.Count(); nIndex++)
                            {
                                if (strLineOfFile[nIndex] == '{')
                                {
                                    nOpenBraceCount++;
                                }
                                else if (strLineOfFile[nIndex] == '}')
                                {
                                    nOpenBraceCount--;

                                    if (nOpenBraceCount == 0)
                                    {
                                        nEndOfMethodIndex = nFileLineIndex;
                                        if (m_dictMethodNameAndFileLocation.ContainsKey(strMethodName)) {
                                            System.Diagnostics.Debugger.Break();
                                            //break;
                                        }
                                        else
                                        {
                                            m_dictMethodNameAndFileLocation.Add(strMethodName, new Tuple<int, int, int>(nStartOfMethodIncludingDocumentationIndex, nStartOfMethodIndex, nEndOfMethodIndex));
                                        }
                                        eSearch = ESearch.SearchingForStartOfMethod;
                                    }
                                }
                            }

                            break;

                        default:

                            break;
                    }
                }

                m_dictOfDictionariesOfMethodNamesAndFileLocations.Add(strRegionName, m_dictMethodNameAndFileLocation);

            }
            else
            {
                return EFileUpdaterStatus.RegionOrPropertyDoesntExist;
            }

            return EFileUpdaterStatus.OK;
        }

        /// <summary>
        /// Checks to see if the Method or Property has already been defined in the Target File.
        /// If so, this allows for copying code from the existing target file into the file that will replace the target file.
        /// This prevents code from being destroyed.
        /// </summary>
        /// <param name="strMethodOrPropertyName">The name of the mathod or property</param>
        /// <param name="nStartingFileLineNumber">The starting File Line Number of the Method or Property</param>
        /// <param name="nEndingFileLineNumber"></param>
        /// <returns></returns>
        public bool DoesMethodOrPropertyAlreadyExistInRegionOfLoadedFile(string strRegionName, string strMethodOrPropertyName, out StringBuilder sbMethodOrPropertyWithDocumentation, out StringBuilder sbMethodOrPropertyWithoutDocumentation, string strExcludeLinesContaining = "")
        {
            bool bReturnValue = false;
            int nIndex;
            int nStartingFileLineNumberWithDocumentation = -1;
            int nStartingFileLineNumberWithoutDocumentation = -1;
            int nEndingFileLineNumber = -1;
            Tuple<int, int, int> tupDocumentationAndStartAndStoppingLineNumbersOfMethod;
            Dictionary<string, Tuple<int, int, int>> dictMethodNameAndFileLocation;
            sbMethodOrPropertyWithDocumentation = new StringBuilder();
            sbMethodOrPropertyWithoutDocumentation = new StringBuilder();

            if (m_dictOfDictionariesOfMethodNamesAndFileLocations.TryGetValue(strRegionName, out dictMethodNameAndFileLocation) == true)
            {
                if (dictMethodNameAndFileLocation.TryGetValue(strMethodOrPropertyName, out tupDocumentationAndStartAndStoppingLineNumbersOfMethod) == true)
                {
                    nStartingFileLineNumberWithDocumentation = tupDocumentationAndStartAndStoppingLineNumbersOfMethod.Item1;
                    nEndingFileLineNumber = tupDocumentationAndStartAndStoppingLineNumbersOfMethod.Item3;

                    for (nIndex = nStartingFileLineNumberWithDocumentation; nIndex <= nEndingFileLineNumber; nIndex++)
                    {
                        if (strExcludeLinesContaining != "")
                        {
                            if (m_astrPrimaryFileContents[nIndex].Contains(strExcludeLinesContaining) == false)
                            {
                                sbMethodOrPropertyWithDocumentation.AppendLine(m_astrPrimaryFileContents[nIndex]);
                            }
                        }
                        else
                        {
                            sbMethodOrPropertyWithDocumentation.AppendLine(m_astrPrimaryFileContents[nIndex]);
                        }
                    }




                    // Now full the Stringbuilder with the Method or Property without documentation
                    nStartingFileLineNumberWithoutDocumentation = tupDocumentationAndStartAndStoppingLineNumbersOfMethod.Item2;
                    nEndingFileLineNumber = tupDocumentationAndStartAndStoppingLineNumbersOfMethod.Item3;

                    for (nIndex = nStartingFileLineNumberWithoutDocumentation; nIndex <= nEndingFileLineNumber; nIndex++)
                    {
                        if (strExcludeLinesContaining != "")
                        {
                            if (m_astrPrimaryFileContents[nIndex].Contains(strExcludeLinesContaining) == false)
                            {
                                sbMethodOrPropertyWithoutDocumentation.AppendLine(m_astrPrimaryFileContents[nIndex]);
                            }
                        }
                        else
                        {
                            sbMethodOrPropertyWithoutDocumentation.AppendLine(m_astrPrimaryFileContents[nIndex]);
                        }
                    }


                    bReturnValue = true;
                }
            }
            return bReturnValue;
        }


        public int GetLinesOfTextFromFile(int nStartingFileLineNumber, int nEndingFileLineNumber, out StringBuilder sbLinesOfText, string strExcludeLinesContaining = "")
        {
            int nIndex;

            sbLinesOfText = new StringBuilder();

            for (nIndex = nStartingFileLineNumber; nIndex <= nEndingFileLineNumber; nIndex++)
            {
                if (strExcludeLinesContaining != "")
                {
                    if (m_astrPrimaryFileContents[nIndex].Contains(strExcludeLinesContaining) == false)
                    {
                        sbLinesOfText.AppendLine(m_astrPrimaryFileContents[nIndex]);
                    }
                    else
                    {

                    }
                }
                else
                {
                    sbLinesOfText.AppendLine(m_astrPrimaryFileContents[nIndex]);
                }
            }

            return 0;
        }

        public int GetLineNumberOfLastLineOfTextInFile()
        {
            return m_astrPrimaryFileContents.Count() - 1;
        }

        public EFileUpdaterStatus GetRegionExtentsInLoadedFile(string strRegionName, out int nStartingFileLineNumber, out int nEndingFileLineNumber)
        {
            Tuple<int, int> tupStartAndStoppingLineNumbersOfRegion;

            nStartingFileLineNumber = -1;
            nEndingFileLineNumber = -1;

            if (m_dictRegionNameAndFileLineNumbers.TryGetValue(strRegionName, out tupStartAndStoppingLineNumbersOfRegion) == true)
            {
                nStartingFileLineNumber = tupStartAndStoppingLineNumbersOfRegion.Item1;
                nEndingFileLineNumber = tupStartAndStoppingLineNumbersOfRegion.Item2;
                return EFileUpdaterStatus.OK;
            }

            return EFileUpdaterStatus.Failed;

        }

        public CFileUpdater(string strFileNameAndPath)
        {
            m_strFileNameAndPath = strFileNameAndPath;

            UpdateStatus();
        }

        public int GetInterfaceLocation(string coreName)
        {
            if (m_eFileUpdaterStatus != EFileUpdaterStatus.OK) return -1;

            var classLineIx = Array.FindIndex(m_astrPrimaryFileContents, l => LineStartsWith(l, new String[] { "public", "interface" }) && l.Contains(coreName));

            return classLineIx;
        }

        public int GetClassLocation(string coreName)
        {
            if (m_eFileUpdaterStatus != EFileUpdaterStatus.OK) return -1;

            var classLineIx = Array.FindIndex(m_astrPrimaryFileContents, l => LineStartsWith(l, new String[] { "public", "class" }) && l.Contains(coreName));

            return classLineIx;
        }

        public int GetConstructorLocation(string coreName)
        {
            if (m_eFileUpdaterStatus != EFileUpdaterStatus.OK) return -1;

            var classLineIx = GetClassLocation(coreName);
            var className = cSharpDeltmrs.Split(m_astrPrimaryFileContents[classLineIx]).Where(l => !String.IsNullOrEmpty(l)).ToArray()[2];
            var constructorLineIx = Array.FindIndex(m_astrPrimaryFileContents, l => LineStartsWith(l, new String[] { "public", className }));

            return constructorLineIx;
        }

        public void InsertLines(int ix, IEnumerable<string> lines)
        {
            if (m_eFileUpdaterStatus != EFileUpdaterStatus.OK) new Exception("File not found or not loaded");

            var file = new List<string>(m_astrPrimaryFileContents);
            file.InsertRange(ix, lines);
            m_astrPrimaryFileContents = file.ToArray();
            File.WriteAllLines(m_strFileNameAndPath, m_astrPrimaryFileContents);
        }

        Regex cSharpDeltmrs = new Regex(@"\s|\{|\(");

        public bool LineStartsWith(string line, String[] tokens)
        {
            if (m_eFileUpdaterStatus != EFileUpdaterStatus.OK) new Exception("File not found or not loaded");

            var result = true;
            var splitLine = cSharpDeltmrs.Split(line).Where(s => !String.IsNullOrEmpty(s)).ToArray();
            if (splitLine.Length < tokens.Length) return false;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (!string.Equals(tokens[i], splitLine[i])) result = false;
                if (result == false) return result;
            }

            return result;
        }


    }
}
