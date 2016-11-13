using System.IO;
using System.Web;
using System.Web.Mvc;
using BackPropagation.BackPropagation;
using System;

namespace BackPropagation.Controllers
{
    public class BackPropController : Controller
    {
        public ActionResult Index()
        {    

            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, string name, string[] dynamicField, Models.InputDataModels form)
        {
            if (dynamicField == null)
            {
                ViewBag.Error = "You MUST add at least one hidden layer!";
                return View();
            }

            // Handle File Upload
            if (name != null && file != null)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Server.MapPath("~/App_Data");

                string newFolder = Path.Combine(path, name);
                if (!Directory.Exists(newFolder))
                {
                    Directory.CreateDirectory(newFolder);
                }

                var newPath = Path.Combine(Server.MapPath("~/App_Data/" + name), fileName);
                file.SaveAs(newPath);

                TempData["FilePath"] = newPath;
            }
            else
            {
                ViewBag.Error = "File upload error!";
                return View();
            }

            // Handle Form Data
            if (form != null)
            {
                TempData["form"] = form;
            }

            // Handle Dynamic Fields with Hidden Neuron Counts
            int i = 0;
            foreach (var field in dynamicField)
            {
                TempData["Item " + i] = field;
                i++;
            }

            TempData["NumItems"] = i;

            return RedirectToAction("CheckData");
        }

        public ActionResult Doc()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CheckData()
        {
            Models.ProcessedDataModels processedData = new Models.ProcessedDataModels();

            string path = TempData["FilePath"] as string;
            int trainPercent = ((Models.InputDataModels)TempData["form"]).trainPercent;

            Loader loader = new Loader(path, trainPercent, ((Models.InputDataModels)TempData["form"]).normData);
            Data trainData = loader.GenerateTrainSet();
            Data testData = loader.GenerateTestSet();

            Topology top = new Topology();
            top.SetInputs(trainData.GetNumFeatures());
            int numClasses = trainData.GetNumClasses();
            testData.GetNumClasses();
            //if(numClasses < 2)
            //{
            //    throw new InvalidDataException();
            //} else if (numClasses == 2)
            //{
            top.SetOutputs(1);
            //} else
            //{
            //    top.SetOutputs(numClasses);
            //}

            int end = ((Models.InputDataModels)TempData["form"]).numHidden;
            ViewBag.NumItems = end;
            for(int i = 0; i < end; ++i)
            {
                ViewData["Item " + i] = TempData["Item " + i];
                top.AddHidden(Convert.ToInt32(TempData["Item " + i]));
            }

            ViewBag.gamma = ((Models.InputDataModels)TempData["form"]).gamma;
            processedData.gamma = ((Models.InputDataModels)TempData["form"]).gamma;
            ViewBag.epsilon = ((Models.InputDataModels)TempData["form"]).epsilon;
            processedData.epsilon = ((Models.InputDataModels)TempData["form"]).epsilon;
            ViewBag.momentum = ((Models.InputDataModels)TempData["form"]).momentum;
            processedData.momentum = ((Models.InputDataModels)TempData["form"]).momentum;
            ViewBag.trPerc = ((Models.InputDataModels)TempData["form"]).trainPercent;
            ViewBag.tsPerc = 100 - (int)((Models.InputDataModels)TempData["form"]).trainPercent;

            ViewBag.numTrain = trainData.GetNumData();
            ViewBag.numTest = testData.GetNumData();

            Network net = new Network(top, true);
            net.SetTrain(trainData);
            net.SetTest(testData);
            processedData.NeuralNet = net;

            TempData["data"] = processedData;

            return View();
        }

        [HttpPost]
        public ActionResult Train()
        {
            if(TempData["data"] != null)
            {
                var data = (Models.ProcessedDataModels)TempData["data"];
                Network net = data.NeuralNet;
                net.Train(data.gamma, data.momentum, data.epsilon);

                // Get Results...
            } else
            {
                ViewBag.Error = "There was an Error!";
            }

            return View();
        }
    }
}