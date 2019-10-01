using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Windows;
using System.Xml;
using Microsoft.Win32;
using VMS.TPS.Common.Model.Types;
using Application = VMS.TPS.Common.Model.API.Application;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VMS.TPS.Common.Model.API;
using System.Text.RegularExpressions;

namespace OutputObjAuto
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                using (Application app = Application.CreateApplication(null, null))
                {
                    Execute(app);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }
        static void Execute(Application app)
        {
            // Read patient list stored in a JSON file 

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;

            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "" && File.Exists(openFileDialog.FileName))
            {
                // Parse JSON
                string text = File.ReadAllText(openFileDialog.FileName);
                PatientList patientList = JsonConvert.DeserializeObject<PatientList>(text);

                // 创建输出文件夹
                if (!Directory.Exists("OutputModels"))
                {
                    Directory.CreateDirectory("OutputModels");
                }

                if (null != patientList?.IdList && patientList.IdList.Count != 0)
                {
                    foreach (string patientId in patientList.IdList)
                    {
                        try
                        {
                            var patientA = app.OpenPatientById(patientId);
                            if (null == patientA)
                            {
                                Console.WriteLine($"Could not open patient {patientId}");
                                continue;
                            }
                            string patientIndex = patientA.Id + "_" + patientA.LastName + patientA.FirstName;
                            Console.Write($"Working with {patientIndex}");

                            // 创建患者名称文件夹
                            string patStrName = Path.Combine("OutputModels", patientIndex);
                            if (!Directory.Exists(patStrName))
                            {
                                Directory.CreateDirectory(patStrName);
                            }

                            // 输出
                            var strTmps = patientA?.StructureSets?.ToArray();
                            if (strTmps == null || strTmps.Length == 0)
                            {
                                Console.WriteLine($"Could not find structure");
                            }
                            else
                            {
                                Regex regex = new Regex(@"[^1-9^a-z^A-Z]+");
                                foreach (StructureSet structureSet in strTmps)
                                {
                                    if (structureSet != null && structureSet.Structures != null && structureSet.Structures.Count() != 0)
                                    {
                                        string validSetId = regex.Replace(structureSet.Id, "_");

                                        // 按照结构集创建文件夹
                                        string strSetName = Path.Combine(patStrName, validSetId);
                                        if (!Directory.Exists(strSetName))
                                        {
                                            Directory.CreateDirectory(strSetName);
                                        }

                                        // 按照不同的结构集进行输出到不同文件夹
                                        foreach (var structure in structureSet.Structures)
                                        {
                                            if (!structure.IsEmpty && structure.HasSegment)
                                            {
                                                MeshOps.ObjWriter obj = new MeshOps.ObjWriter(structure.MeshGeometry, null);
                                                obj.OutPut(Path.Combine(strSetName, structure.Id + ".obj"), "structure" + structure.Id);
                                            }
                                        }
                                    }
                                }
                            }
                            
                            Console.WriteLine($", Finished");

                            app.ClosePatient();
                        }
                        catch (Exception e)
                        {
                            app.ClosePatient();
                            Console.WriteLine(e);
                            Console.WriteLine($"Error, deal with patient {patientId}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Empty patient list");
                }
            }
            else
            {
                Console.WriteLine("Please specified the patient list JSON file");

                return;
            }
        }
    }

    // 
    public class PatientList
    {
        public List<String> IdList = new List<string>();
    }
}
