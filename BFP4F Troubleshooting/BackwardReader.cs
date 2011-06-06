using System.IO;
using System.Text;

/*
 * BackwardReader class in reply to: "Problem reading a text file from the end to the beginning C#"
 * Author: the_real_herminator
 * URL: http://social.msdn.microsoft.com/forums/en-US/csharpgeneral/thread/9acdde1a-03cd-4018-9f87-6e201d8f5d09#3cd342b9-38f1-4a7b-8aca-af301fc7a868
 */

namespace BFP4F_Troubleshooting
{
    class BackwardReader
    {
        #region Fields

        private string _path = "";
        private FileStream _stream = null;

        #endregion


        #region Properties

        public bool SOF
        {
            get { return this._stream.Position == 0; }
        }

        #endregion


        #region Constructor(s)

        public BackwardReader(string path)
        {
            this._path = path;
            this._stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            this._stream.Seek(0, SeekOrigin.End);
        }

        #endregion


        #region Methods

        public string ReadLine()
        {
            byte[] line;
            byte[] text = new byte[1];
            long position = 0;
            int count;

            this._stream.Seek(0, SeekOrigin.Current);
            position = this._stream.Position;

            //do we have trailing \r\n?
            if (this._stream.Length > 1)
            {
                byte[] vagnretur = new byte[2];
                this._stream.Seek(-2, SeekOrigin.Current);
                this._stream.Read(vagnretur, 0, 2);

                if (ASCIIEncoding.ASCII.GetString(vagnretur).Equals("\r\n"))
                {
                    //move it back
                    this._stream.Seek(-2, SeekOrigin.Current);
                    position = this._stream.Position;
                }
            }

            while (this._stream.Position > 0)
            {
                text.Initialize();

                //read one char
                this._stream.Read(text, 0, 1);
                string asciiText = ASCIIEncoding.ASCII.GetString(text);

                //moveback to the charachter before
                this._stream.Seek(-2, SeekOrigin.Current);

                if (asciiText.Equals("\n"))
                {
                    this._stream.Read(text, 0, 1);
                    asciiText = ASCIIEncoding.ASCII.GetString(text);
                    if (asciiText.Equals("\r"))
                    {
                        this._stream.Seek(1, SeekOrigin.Current);
                        break;
                    }
                }
            }

            count = int.Parse((position - this._stream.Position).ToString());
            line = new byte[count];
            this._stream.Read(line, 0, count);
            this._stream.Seek(-count, SeekOrigin.Current);

            return ASCIIEncoding.ASCII.GetString(line);
        }

        public void Close()
        {
            this._stream.Close();
        }

        #endregion
    }
}
