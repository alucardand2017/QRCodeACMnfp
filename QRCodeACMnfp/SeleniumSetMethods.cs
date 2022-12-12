using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
namespace QRCodeACMnfp
{
    class SeleniumSetMethods
    {
        public static void EnterText(IWebDriver driver, string cnpj, string data, string valor, string extrato)
        {
            
            IWebElement webElementCnpf = driver.FindElement(By.Id("divCNPJEstabelecimento"));
            IWebElement campoCnpj = webElementCnpf.FindElement(By.TagName("input"));
            
            IWebElement webElementDaTa = driver.FindElement(By.Id("divtxtDtNota"));
            IWebElement campodata = webElementDaTa.FindElement(By.TagName("input"));


            IWebElement webElementExtrato = driver.FindElement(By.Id("divtxtNrNota"));
            IWebElement campoExtrato = webElementExtrato.FindElement(By.TagName("input"));

            IWebElement webElementValor = driver.FindElement(By.Id("divtxtVlNota"));
            IWebElement campoValor = webElementValor.FindElement(By.TagName("input"));



            campoCnpj.Clear();
            campodata.Clear();
            campoExtrato.Clear();
            campoValor.Clear();


            campoCnpj.SendKeys(cnpj);
            campodata.SendKeys(data);
            campoExtrato.SendKeys(extrato);
            campoValor.SendKeys(valor);


        }

        public static void Click(IWebDriver driver, string element, string value, string elementtype)
        {
            if (elementtype == "Id")
                driver.FindElement(By.Id(element)).Click();
            if (elementtype == "Name")
                driver.FindElement(By.Name(elementtype)).Click();
        }

        public static void SelectDropDown(IWebDriver driver, string element, string value, string elementtype)
        {
           
            if (elementtype == "Id")
           new SelectElement(driver.FindElement(By.Id(element))).SelectByText(value);
            if (elementtype == "Name")
                new SelectElement(driver.FindElement(By.Name(element))).SelectByText(value);
        }
    }
}
