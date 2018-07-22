using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using XMLDatabaseInterface.Core;

namespace XMLDatabaseInterface.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IDataProvider, XmlDataProvider>();
            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<CommonNamesViewModel>();
            SimpleIoc.Default.Register<CommonSurenamesViewModel>();
            SimpleIoc.Default.Register<BirthdayTodayViewModel>();
        }

        public MainWindowViewModel Main => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
        public CommonNamesViewModel CommonNames => ServiceLocator.Current.GetInstance<CommonNamesViewModel>();
        public CommonSurenamesViewModel CommonSurenames => ServiceLocator.Current.GetInstance<CommonSurenamesViewModel>();
        public BirthdayTodayViewModel BirthdayToday => ServiceLocator.Current.GetInstance<BirthdayTodayViewModel>();


        public static void Cleanup()
        {
            SimpleIoc.Default.Reset();
        }
    }
}