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

            Loader loader = new Loader(path, trainPercent);
            processedData.TrainData = loader.GenerateTrainSet();
            processedData.TestData = loader.GenerateTestSet();

            Topology top = new Topology();
            top.SetInputs(processedData.TrainData.GetNumFeatures());
            int numClasses = processedData.TrainData.GetNumClasses();
            if(numClasses < 2)
            {
                throw new InvalidDataException();
            } else if (numClasses == 2)
            {
                top.SetOutputs(1);
            } else
            {
                top.SetOutputs(numClasses);
            }

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
            ViewBag.trPerc = ((Models.InputDataModels)TempData["form"]).trainPercent;
            ViewBag.tsPerc = 100 - (int)((Models.InputDataModels)TempData["form"]).trainPercent;

            ViewBag.numTrain = processedData.TrainData.GetNumData();
            ViewBag.numTest = processedData.TestData.GetNumData();

            TempData["data"] = processedData;

            return View();
        }
    }
}