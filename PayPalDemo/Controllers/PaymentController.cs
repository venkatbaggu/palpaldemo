using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PayPalDemo.Controllers
{
    public class PaymentController : Controller
    {
        //
        // GET: /Payment/
        public ActionResult Index()
        {
            if (ModelState.IsValid)
            {
                //API Credentials (3-token)
                string strUsername = ConfigurationManager.AppSettings["InStoreAPIUsername"].ToString();
                string strPassword = ConfigurationManager.AppSettings["InStoreAPIPassword"].ToString();
                string strSignature = ConfigurationManager.AppSettings["InStoreAPISignature"].ToString();
                string strCredentials = "USER=" + strUsername + "&PWD=" + strPassword + "&SIGNATURE=" + strSignature;


                //SandBox server Url - https://api-3t.paypal.com/nv
                //Live server Url - https://api.paypal.com/nvp

                //strNVPSandboxServer  represents server url
                string strNVPSandboxServer = ConfigurationManager.AppSettings["NVPSandboxServer"].ToString();

                string strAPIVersion = "60.0";


                //Yoyr IP Address
                string ipAddress = "xx.xxx.xxx.xxx";

                //sId represents the stateId
                //NY - NewYork
                var stateName = "NY";



                //grandTotal represents the shopping cart total 
                string grandTotal = "10.00";
                var expirationMonth = "12";
                var expirationYear = "31";
                var billingFirstName = "John";
                var billingLastName = "Doe";
                var billingAddress1 = "123 Main Street";


                string strNVP = "METHOD=DoDirectPayment" +
                            "&VERSION=" + strAPIVersion +
                            "&PWD=" + strPassword +
                            "&USER=" + strUsername +
                            "&SIGNATURE=" + strSignature +
                            "&PAYMENTACTION=Sale" +
                            "&IPADDRESS=82.69.92.153" +
                            "&RETURNFMFDETAILS=0" +
                            "&CREDITCARDTYPE=" + "Visa" +
                            "&ACCT=" + "4111111111111111" +
                            "&EXPDATE=" + expirationMonth + expirationYear +
                            "&CVV2=" + 111 +
                            "&STARTDATE=" +
                            "&ISSUENUMBER=" +
                            "&EMAIL=youremail@gmail.com" +
                            //the following  represents the billing details
                            "&FIRSTNAME=" + billingFirstName +
                            "&LASTNAME=" + billingLastName +
                            "&STREET=" + billingAddress1 +
                            "&STREET2=" + "" +
                            "&CITY=" + "Memphsis" +
                            "&STATE=" + "TN" +
                            "&COUNTRYCODE=US" +
                            "&ZIP=" + "38134" +
                            "&AMT=" + "100.00" +//orderdetails.GrandTotal.ToString("0.0")+
                            "&CURRENCYCODE=USD" +
                            "&DESC=Test Sale Tickets" +
                            "&INVNUM=" + "";




                try
                {
                    //Create web request and web response objects, make sure you using the correct server (sandbox/live)
                    HttpWebRequest wrWebRequest = (HttpWebRequest)WebRequest.Create(strNVPSandboxServer);
                    wrWebRequest.Method = "POST";
                    StreamWriter requestWriter = new StreamWriter(wrWebRequest.GetRequestStream());
                    requestWriter.Write(strNVP);
                    requestWriter.Close();

                    // Get the response.
                    HttpWebResponse hwrWebResponse = (HttpWebResponse)wrWebRequest.GetResponse();
                    StreamReader responseReader = new StreamReader(wrWebRequest.GetResponse().GetResponseStream());

                    //and read the response
                    string responseData = responseReader.ReadToEnd();
                    responseReader.Close();

                    string result = Server.UrlDecode(responseData);

                    string[] arrResult = result.Split('&');
                    Hashtable htResponse = new Hashtable();
                    string[] responseItemArray;
                    foreach (string responseItem in arrResult)
                    {
                        responseItemArray = responseItem.Split('=');
                        htResponse.Add(responseItemArray[0], responseItemArray[1]);
                    }

                    string strAck = htResponse["ACK"].ToString();

                    //strAck = "Success";

                    if (strAck == "Success" || strAck == "SuccessWithWarning")
                    {
                        string strAmt = htResponse["AMT"].ToString();
                        string strCcy = htResponse["CURRENCYCODE"].ToString();
                        string strTransactionID = htResponse["TRANSACTIONID"].ToString();
        
                        //SaveOrder();
                        //Send Email

                        string strSuccess = "Thank you, your order for: $" + strAmt + " " + strCcy + " has been processed.";
    

                        ViewBag.Amount = strAmt;
                        ViewBag.Currency = strCcy;


                    }
                    else
                    {
                        ViewBag.Error = "Error: " + htResponse["L_LONGMESSAGE0"].ToString();
                        ViewBag.ErrorCode = "Error code: " + htResponse["L_ERRORCODE0"].ToString();
                        
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                ViewBag.Failure = "failure";
                return View();
            }
            return View();
        }

       
	}
}