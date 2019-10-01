using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace OutputObjAuto
{
    public class MeshOps
    {
        // ref 
        public class ObjWriter
        {
            MeshGeometry3D _model;
            BitmapSource _texture;

            const string SPLITER = "#==================================================================================================";
            const string STR_BETWEEN_BLOCKS = "\r\n\r\n\r\n\r\n";

            public ObjWriter(MeshGeometry3D modle, BitmapSource texture)
            {
                _model = modle;
                _texture = texture;
            }

            public void OutPut(string fileName, string fileSafeName)
            {
                CreatObj(fileName, fileSafeName);
                // CreatMtl(fileName, fileSafeName);
                // CreatReadMe(fileName, fileSafeName);
                // CreatTexture(fileName);
            }
            public void CreatReadMe(string fileName, string fileSafeName)
            {
                FileStream fileStream = new FileStream(fileName + "Read Me.txt", FileMode.Create, FileAccess.ReadWrite);
                StreamWriter streamWriter = new StreamWriter(fileStream);

                streamWriter.WriteLine(SPLITER);
                streamWriter.WriteLine("#");
                streamWriter.WriteLine("#  Those files listed below is auto generated. Please DO NOT edit them !  ");
                streamWriter.WriteLine("#  You can run the first file in meshlab or 3dmax . But you MUST have all those THREE files !");
                streamWriter.WriteLine("#  If you have more question please send e-mail to \"zhao_chenhui_scu@163.com\".");
                streamWriter.WriteLine("#");
                streamWriter.WriteLine("#  1. " + fileSafeName);
                streamWriter.WriteLine("#  2. " + fileSafeName + ".bmp");
                streamWriter.WriteLine("#  3. " + fileSafeName + ".mtl");
                streamWriter.WriteLine("#");
                streamWriter.WriteLine("#  Thank you ! And have a nice day !");
                streamWriter.WriteLine("#  Generated Time : " + DateTime.Now.ToString());
                streamWriter.WriteLine(SPLITER);

                streamWriter.Flush();
                streamWriter.Close();
                fileStream.Close();
            }

            private void CreatTexture(string fileName)
            {
                FileStream fileStream = new FileStream(fileName + ".bmp", FileMode.Create, FileAccess.ReadWrite);

                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(_texture));
                encoder.Save(fileStream);

                fileStream.Close();
            }

            private void CreatMtl(string fileName, string fileSafeName)
            {
                FileStream fileStream = new FileStream(fileName + ".mtl", FileMode.Create, FileAccess.ReadWrite);
                StreamWriter streamWriter = new StreamWriter(fileStream);

                streamWriter.WriteLine(SPLITER);
                streamWriter.WriteLine("#");
                streamWriter.WriteLine("# Warming        : This file is auto generated . Please DO NOT edit it ! ");
                streamWriter.WriteLine("#");
                streamWriter.WriteLine("# Generated Time : " + DateTime.Now.ToString());
                streamWriter.WriteLine("#");
                streamWriter.WriteLine("# Description    :You can open this file in meshlab or 3dmax .");
                streamWriter.WriteLine("#                 If you have more question please send e-mail to \"zhao_chenhui_scu@163.com\".");
                streamWriter.WriteLine("#                 This file MUST work with other two files named:\"" + fileSafeName + "\" and \"" + fileSafeName + ".bmp\".");
                streamWriter.WriteLine("#");
                streamWriter.WriteLine(SPLITER);
                streamWriter.WriteLine(STR_BETWEEN_BLOCKS);

                streamWriter.WriteLine("newmtl mt1");
                streamWriter.WriteLine("Ka 0.0000 0.0000 0.0000");
                streamWriter.WriteLine("Kd 0.0000 0.0000 0.0000");
                streamWriter.WriteLine("Ks 0.0000 0.0000 0.0000");
                streamWriter.WriteLine("Ke 0.9725 0.9725 0.9725");

                streamWriter.WriteLine("map_Ka " + fileSafeName + ".bmp");
                streamWriter.WriteLine("map_Kd " + fileSafeName + ".bmp");


                streamWriter.Flush();
                streamWriter.Close();
                fileStream.Close();
            }

            private void CreatObj(string fileName, string fileSafeName)
            {
                FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                StreamWriter streamWriter = new StreamWriter(fileStream);

                streamWriter.WriteLine(GetHeadString(fileSafeName));
                streamWriter.WriteLine(STR_BETWEEN_BLOCKS);

                // streamWriter.WriteLine(GetMatralString(fileSafeName));
                // streamWriter.WriteLine(STR_BETWEEN_BLOCKS);

                streamWriter.Write(GetVerticesString());
                streamWriter.WriteLine(STR_BETWEEN_BLOCKS);

                // streamWriter.Write(GetTexturePointString());
                // streamWriter.WriteLine(STR_BETWEEN_BLOCKS);

                streamWriter.Write(GetFaceString());

                streamWriter.Flush();
                streamWriter.Close();
                fileStream.Close();
            }

            private string GetHeadString(string fileSafeName)
            {
                StringBuilder result = new StringBuilder();

                result.AppendLine(SPLITER);
                result.AppendLine("#");
                result.AppendLine("# Warming        : This file is auto generated . Please DO NOT edit it ! ");
                result.AppendLine("#");
                result.AppendLine("# Generated Time : " + DateTime.Now.ToString());
                result.AppendLine("#");
                result.AppendLine("# Description    :You can open this file in meshlab or 3dmax .");
                result.AppendLine("#");
                result.AppendLine(SPLITER);

                return result.ToString();
            }

            private string GetMatralString(string fileSafeName)
            {
                StringBuilder result = new StringBuilder();

                result.AppendLine(SPLITER + "\r\n");
                result.AppendLine("#  Matral : 1  \r\n");
                result.AppendLine("mtllib " + fileSafeName + ".mtl");
                result.AppendLine("\r\n#  Matral End \r\n");
                result.AppendLine(SPLITER);

                return result.ToString();
            }

            private string GetVerticesString()
            {
                StringBuilder result = new StringBuilder();

                result.AppendLine(SPLITER + "\r\n");
                result.AppendLine("#  Vertices: " + _model.Positions.Count + "\r\n");

                foreach (Point3D p in _model.Positions)
                    result.AppendLine("v " + p.X + " " + p.Y + " " + p.Z);

                result.AppendLine("\r\n#  Vertices End");
                result.AppendLine("\r\n" + SPLITER);

                return result.ToString();
            }

            private string GetTexturePointString()
            {
                StringBuilder result = new StringBuilder();

                result.AppendLine(SPLITER + "\r\n");
                result.AppendLine("#  Texture Coordinates: " + _model.TextureCoordinates.Count + "\r\n");

                foreach (Point p in _model.TextureCoordinates)
                    result.AppendLine("vt " + p.X + " " + p.Y + " 0");


                result.AppendLine("\r\n#  Texture Coordinates End");
                result.AppendLine("\r\n" + SPLITER);

                return result.ToString();
            }

            private string GetFaceString()
            {
                StringBuilder result = new StringBuilder();

                result.AppendLine(SPLITER + "\r\n");
                result.AppendLine("#  Faces: " + _model.TriangleIndices.Count / 3 + "\r\n");
                result.AppendLine("usemtl mt1" + "\r\n");

                for (int i = 0; i < _model.TriangleIndices.Count - 1; i += 3)
                {

                    result.Append("f ");

                    int p1 = _model.TriangleIndices[i] + 1;
                    int p2 = _model.TriangleIndices[i + 1] + 1;
                    int p3 = _model.TriangleIndices[i + 2] + 1;

                    // result.Append(p1 + "/" + p1 + " ");
                    // result.Append(p2 + "/" + p2 + " ");
                    // result.Append(p3 + "/" + p3 + " ");
                    result.Append(p1 + " ");
                    result.Append(p2 + " ");
                    result.Append(p3 + " ");

                    result.AppendLine();
                }

                result.AppendLine("\r\n#  Faces End");
                result.AppendLine("\r\n" + SPLITER);

                return result.ToString();
            }
        }

    }
}
