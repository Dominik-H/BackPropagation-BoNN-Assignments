using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using CohonenNet;

namespace BackPropagation.Controllers
{
    public class CohonenController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, string name, Models.CohonenDataModels form)
        {
            // Handle File Upload
            if (name != null && file != null)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Server.MapPath("~/App_Data/Cohonen");

                string newFolder = Path.Combine(path, name);
                if (!Directory.Exists(newFolder))
                {
                    Directory.CreateDirectory(newFolder);
                }

                var newPath = Path.Combine(Server.MapPath("~/App_Data/Cohonen/" + name), fileName);
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

            return RedirectToAction("Results");
        }

        public ActionResult Doc()
        {
            return View();
        }

        public ActionResult Results()
        {
            string path = TempData["FilePath"] as string;
            Models.CohonenDataModels form = (Models.CohonenDataModels)TempData["form"];

            BitmapLoader load = null;
            Pixels data;
            Network net;
            Bitmap img = null;

            Bitmap loaded = new Bitmap(path, true);
            img = new Bitmap(loaded.Width, loaded.Height);
            using (var g = Graphics.FromImage(img))
            {
                g.DrawImage(loaded, 0, 0);
            }
            load = new BitmapLoader(img);

            if (load != null && img != null)
            {
                data = load.data;
                net = new Network(form.gamma, form.radius, load.mid, form.numX, form.numY, form.numIter, data);
                net.Train();
                List<List<Neuron>> output = net.GetNet();

                Pen rPen = new Pen(Color.Red, 1);
                SolidBrush rBrush = new SolidBrush(Color.Red);

                var g = Graphics.FromImage(img);
                foreach (var l in output)
                {
                    foreach (var n in l)
                    {
                        g.FillEllipse(rBrush, new Rectangle((int)n.weightX - 2, (int)n.weightY - 2, 4, 4));
                    }
                }

                for (int i = 1; i <= form.numX; ++i)
                {
                    for (int j = 1; j < form.numY; ++j)
                    {
                        Neuron n = net.GetNeuron(i, j);
                        if (i != form.numX)
                        {
                            Neuron rn = net.GetNeuron(i + 1, j);
                            Neuron bn = net.GetNeuron(i, j + 1);
                            g.DrawLine(rPen, new Point((int)n.weightX, (int)n.weightY), new Point((int)rn.weightX, (int)rn.weightY));
                            g.DrawLine(rPen, new Point((int)n.weightX, (int)n.weightY), new Point((int)bn.weightX, (int)bn.weightY));
                        }
                        else
                        {
                            Neuron bn = net.GetNeuron(i, j + 1);
                            g.DrawLine(rPen, new Point((int)n.weightX, (int)n.weightY), new Point((int)bn.weightX, (int)bn.weightY));
                        }
                    }
                }

                for (int i = 1; i < form.numX; ++i)
                {
                    Neuron n = net.GetNeuron(i, form.numY);
                    Neuron rn = net.GetNeuron(i + 1, form.numY);
                    g.DrawLine(rPen, new Point((int)n.weightX, (int)n.weightY), new Point((int)rn.weightX, (int)rn.weightY));
                }

                var outPath = Path.Combine(Server.MapPath("~/App_Data/Cohonen"), "out.png");
                img.Save(outPath, ImageFormat.Png);
                ViewBag.image = outPath;
            }

            return View();
        }
    }
}