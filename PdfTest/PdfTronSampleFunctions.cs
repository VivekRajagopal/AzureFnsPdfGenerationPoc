using System;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using pdftron.PDF;
using System.Collections.Generic;

namespace PdfTest
{
    public class PdfTronSampleFunctions
    {
        [FunctionName("GetPdf_PdfTron")]
        public FileStreamResult GetPdf_PdfTron([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "ssc-form.pdf");

            var pdfContent = GetStandardChoiceForm(path, req.Query["tfn"]);
            return new FileStreamResult(pdfContent, "application/pdf");
        }

        [FunctionName("GetPdf_PdfTron_Filled")]
        public Field[] GetPdf_PdfTron_Filled([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "ssc-form-filled.pdf");

            return GetFormFields(path);
        }

        private Stream GetStandardChoiceForm(string sourcePath, string tfn)
        {
            var fileStream = File.OpenRead(sourcePath);

            using (var pdfDoc = new PDFDoc(fileStream))
            {
                pdfDoc.GetField("2-TFN-fill").SetValue(tfn ?? "000 000 000");
                pdfDoc.RefreshFieldAppearances();
                pdfDoc.FlattenAnnotations();

                pdfDoc.Save(pdftron.SDF.SDFDoc.SaveOptions.e_linearized);

                return new MemoryStream(pdfDoc.Save(pdftron.SDF.SDFDoc.SaveOptions.e_linearized));
            }
        }

        private Field[] GetFormFields(string sourcePath)
        {
            var pdfDoc = new PDFDoc(sourcePath);

            var formFields = new List<Field>();
            var itr = pdfDoc.GetFieldIterator();

            for (; itr.HasNext(); itr.Next())
            {
                formFields.Add(itr.Current());
            }

            return formFields.ToArray();
        }
    }
}