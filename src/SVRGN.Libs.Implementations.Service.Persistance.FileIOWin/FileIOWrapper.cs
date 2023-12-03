using SVRGN.Libs.Contracts.Service.Logging;
using SVRGN.Libs.Contracts.Service.Persistance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SVRGN.Libs.Implementations.Service.Persistance.FileIOWin
{
    public class FileIOWrapper : IFileIOWrapper
    {
        #region Properties

        private ILogService logService;

        #endregion Properties

        #region Construction

        public FileIOWrapper(ILogService LogService)
        {
            this.logService = LogService;
        }

        #endregion Construction

        #region Methods

        #region Exists
        public bool Exists(string path)
        {
            return File.Exists(path);
        }
        #endregion Exists

        #region ReadAllText
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
        #endregion ReadAllText

        #region WriteAllText
        public bool WriteAllText(string path, string text)
        {
            bool result = false;
            try
            {
                File.WriteAllText(path, text);
                result = true;
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        #endregion WriteAllText

        #region DirectoryGetFiles
        public List<string> DirectoryGetFiles(string Path)
        {
            List<string> result = new List<string>();
            string[] filenames = Directory.GetFiles(Path);

            // remove absolute path
            foreach (string filename in filenames)
            {
                result.Add(System.IO.Path.GetFileName(filename));
            }
            return result;
        }
        #endregion DirectoryGetFiles

        #region ReadStream
        public Stream ReadStream(string Filename)
        {
            Stream result = default;
            //read file stream
            System.IO.MemoryStream targetStream = new System.IO.MemoryStream();
            using (System.IO.Stream sourceStream = File.OpenRead(Filename))  // use using for automated closing of the file
            {
                // https://stackoverflow.com/questions/8741474/returning-a-stream-from-file-openread
                sourceStream.CopyTo(targetStream);
                targetStream.Position = 0;
                byte[] buf = new byte[targetStream.Length];
                targetStream.Read(buf, 0, buf.Length);
            }
            result = targetStream;
            return result;
        }
        #endregion ReadStream

        #region WriteStream
        public bool WriteStream(string Filename, Stream Input)
        {
            bool result = false;

            this.CreateDirectory(Filename);

            // change the input stream to memory stream to make sure, seek is supported
            MemoryStream newStream = new MemoryStream();
            Input.CopyTo(newStream);

            if (Input != null)
            {
                using (var fileStream = File.Create(Filename))
                {
                    newStream.Seek(0, SeekOrigin.Begin);
                    newStream.CopyTo(fileStream);
                }
            }
            else
            {
                this.logService.Error("FileIOWrapper", "WriteStream", $"Stream Input was null (perhaps by casting?)");
            }

            return result;
        }
        #endregion WriteStream

        #region CreateDirectory: creates a directory if not existing yet
        /// <summary>
        /// creates a directory if not existing yet
        /// </summary>
        /// <param name="FilenameOrPath">the file name or path ro ensure that the directory is there</param>
        private void CreateDirectory(string FilenameOrPath)
        {
            string directory = Path.GetDirectoryName(FilenameOrPath);

            Directory.CreateDirectory(directory);
        }
        #endregion CreateDirectory

        #endregion Methods
    }
}
