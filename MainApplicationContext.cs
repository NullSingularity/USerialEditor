using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USerialEditor
{
    public class MainApplicationContext : ApplicationContext
    {
        // FormTermDisplay termDisplay;
        // FormFileDisplay fileDisplay;
        FormMain mainForm;
        FormSourceManager sourceManagerForm;

        PropertyFactory propertyFactory;
        ClassLibrary classLibrary;
        SourceCodeLibrary sourceCodeLibrary;
        SourceTemplateProcessor sourceTemplateProcessor;
        SourceCodeProcessor sourceCodeProcessor;
        SourceLoader sourceLoader;
        private void onFormClosed(object sender, EventArgs e)
        {
            if (sender == mainForm)//Application.OpenForms.Count == 0)
            {
                ExitThread();
            }
        }

        public MainApplicationContext() 
        {
            InitBackend();
            TEMPLoadTemplates();
            InitForms();
            mainForm.Show();
            //sourceManagerForm.Show();
        }

        public void TEMPLoadTemplates()
        {
            sourceLoader.LoadSourceFile("X:\\SteamLibrary\\steamapps\\common\\HatinTime\\HatinTimeGame\\Mods\\2D_Game\\Classes\\SaveTest.uc");
            sourceLoader.LoadSourceFile("X:\\SteamLibrary\\steamapps\\common\\HatinTime\\HatinTimeGame\\Mods\\2D_Game\\Classes\\InheritTest.uc");
            sourceLoader.LoadSourceDirectory("X:\\SteamLibrary\\steamapps\\common\\HatinTime\\Development\\Src\\Core\\Classes");
            sourceLoader.LoadSourceDirectory("X:\\SteamLibrary\\steamapps\\common\\HatinTime\\Development\\Src\\Engine\\Classes");
            sourceLoader.LoadSourceDirectory("X:\\SteamLibrary\\steamapps\\common\\HatinTime\\Development\\Src\\HatinTimeGame\\Classes");
            sourceLoader.LoadSourceDirectory("X:\\SteamLibrary\\steamapps\\common\\HatinTime\\Development\\Src\\HatinTimeGameContent\\Classes");
            sourceLoader.LoadSourceDirectory("X:\\SteamLibrary\\steamapps\\common\\HatinTime\\Development\\Src\\HatinTimeGameContent\\Classes\\Players");
            sourceLoader.LoadSourceDirectory("X:\\SteamLibrary\\steamapps\\common\\HatinTime\\Development\\Src\\HatinTimeGameContent\\Classes\\NPCs");
            sourceCodeProcessor.ReloadClassTemplates();
        }

        private void InitBackend() 
        {
            

            classLibrary = new ClassLibrary();
            propertyFactory = new PropertyFactory(classLibrary);
            propertyFactory.InitDefaultPropertyConstructors();
            sourceCodeLibrary = new SourceCodeLibrary();
            sourceTemplateProcessor = new SourceTemplateProcessor(propertyFactory);
            sourceCodeProcessor = new SourceCodeProcessor(sourceCodeLibrary, sourceTemplateProcessor, classLibrary);
            sourceLoader = new SourceLoader(sourceCodeProcessor, sourceCodeLibrary);
        }

        private void InitForms()
        {
            List<Form> forms = new List<Form>();
           // fileDisplay = new FormFileDisplay("","");
           // termDisplay = new FormTermDisplay(fileDisplay, propertyFactory);
           // forms.Add(fileDisplay); 
           // forms.Add(termDisplay);
            mainForm = new FormMain(this,classLibrary, sourceCodeProcessor, propertyFactory);
            sourceManagerForm = new FormSourceManager(sourceLoader, sourceCodeLibrary, sourceCodeProcessor);
            forms.Add(mainForm);
            forms.Add(sourceManagerForm);
            foreach (Form f in forms) 
            {
                f.FormClosed += onFormClosed;
            }
        }

        public void ShowSourceManager()
        {
            if(sourceManagerForm != null) 
            {
                sourceManagerForm.Show();
            }
        }
    }
}
