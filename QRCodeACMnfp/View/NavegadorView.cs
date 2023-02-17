using OpenQA.Selenium;
using System;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;
using QRCodeACMnfp.Domain;


namespace QRCodeACMnfp
{
    class NavegadorView
    {

        public static IWebDriver DriverChrome { get; set; } = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        //public static void EnterText(IWebDriver driver, string cnpj, string data, string valor, string extrato)
        //{
        //    try
        //    {
        //        IWebElement webElementCnpf = driver.FindElement(By.Id("divCNPJEstabelecimento"));
        //        IWebElement campoCnpj = webElementCnpf.FindElement(By.TagName("input"));

        //        IWebElement webElementDaTa = driver.FindElement(By.Id("divtxtDtNota"));
        //        IWebElement campodata = webElementDaTa.FindElement(By.TagName("input"));


        //        IWebElement webElementExtrato = driver.FindElement(By.Id("divtxtNrNota"));
        //        IWebElement campoExtrato = webElementExtrato.FindElement(By.TagName("input"));

        //        IWebElement webElementValor = driver.FindElement(By.Id("divtxtVlNota"));
        //        IWebElement campoValor = webElementValor.FindElement(By.TagName("input"));



        //        campoCnpj.Clear();
        //        campodata.Clear();
        //        campoExtrato.Clear();
        //        campoValor.Clear();


        //        campoCnpj.SendKeys(cnpj);
        //        campodata.SendKeys(data);
        //        campoExtrato.SendKeys(extrato);
        //        campoValor.SendKeys(valor);


        //    }
        //    catch (WebDriverArgumentException)
        //    {
        //        throw new WebDriverArgumentException("Erro no mapeamento e inserção dos elementos da página Web " +
        //            "no sistema da Nota fiscal paulista"); ;
        //    }
        //}
        internal static void EnterText(IWebDriver driverChrome, Formulario formulario)
        {
            try
            {
                IWebElement webElementCnpf = driverChrome.FindElement(By.Id("divCNPJEstabelecimento"));
                IWebElement campoCnpj = webElementCnpf.FindElement(By.TagName("input"));

                IWebElement webElementDaTa = driverChrome.FindElement(By.Id("divtxtDtNota"));
                IWebElement campodata = webElementDaTa.FindElement(By.TagName("input"));


                IWebElement webElementExtrato = driverChrome.FindElement(By.Id("divtxtNrNota"));
                IWebElement campoExtrato = webElementExtrato.FindElement(By.TagName("input"));

                IWebElement webElementValor = driverChrome.FindElement(By.Id("divtxtVlNota"));
                IWebElement campoValor = webElementValor.FindElement(By.TagName("input"));



                campoCnpj.Clear();
                campodata.Clear();
                campoExtrato.Clear();
                campoValor.Clear();


                campoCnpj.SendKeys(formulario.Cnpj.Text);
                campodata.SendKeys(formulario.Data.Text);
                campoExtrato.SendKeys(formulario.Extrato.Text);
                campoValor.SendKeys(formulario.Valor.Text.Replace(".", "").Replace(",", ""));


            }
            catch (WebDriverArgumentException)
            {
                throw new WebDriverArgumentException("Erro no mapeamento e inserção dos elementos da página Web " +
                    "no sistema da Nota fiscal paulista"); ;
            }
        }
        public static void Click(IWebDriver driver, string element, string value, string elementtype)
        {
            try
            {
                if (elementtype == "input")
                    driver.FindElement(By.Id(element)).Click();
            }
            catch (Exception)
            {
                throw new WebDriverArgumentException("Não foi possível encontra o CTA da página.");
            }
        }
        public static void Click(IWebDriver driver)
        {
            try
            {
                driver.FindElement(By.Id("btnSalvarNota")).Click();
            }
            catch (Exception)
            {
                throw new WebDriverArgumentException("Não foi possível encontra o CTA da página.");
            }
        }
        //public static void SelectDropDown(IWebDriver driver, string element, string value, string elementtype)
        //{
        //    try
        //    {
        //        if (elementtype == "Id")
        //            new SelectElement(driver.FindElement(By.Id(element))).SelectByText(value);
        //        if (elementtype == "Name")
        //            new SelectElement(driver.FindElement(By.Name(element))).SelectByText(value);
        //    }
        //    catch (WebDriverArgumentException)
        //    {
        //        throw new WebDriverArgumentException("Não foi possível encontrar o campo que define o tipo de cupom" +
        //            "fiscal na página.");
        //    }
        //}

        internal static void SelectDropDown(IWebDriver driverChrome)
        {
            try
            {
                new SelectElement(driverChrome.FindElement(By.Id("ddlTpNota"))).SelectByText("Cupom Fiscal");
            }
            catch (WebDriverArgumentException)
            {
                throw new WebDriverArgumentException("Não foi possível encontrar o campo que define o tipo de cupom" +
                    "fiscal na página.");
            }
        }
    }
}
