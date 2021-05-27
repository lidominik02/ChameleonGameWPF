using ChameleonGameWPF.Model;
using ChameleonGameWPF.Persistence;
using CameleonWPF.View;
using ChameleonGameWPF.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CameleonWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ChameleonModel _model;
        private ChameleonViewModel _viewModel;
        private MainWindow _view;

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new ChameleonModel(new ChameleonFileDataAccess());
            _model.GameOver += new EventHandler<ChameleonEventArgs>(Model_GameOver);

            _viewModel = new ChameleonViewModel(_model);
            _viewModel.NewGame += new EventHandler<int>(ViewModel_NewGame);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGameAsync);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGameAsync);
            _viewModel.InvalidStep += new EventHandler(ViewModel_InvalidStep);

            _model.NewGame(3);

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Show();
        }

        private void ViewModel_InvalidStep(object sender, EventArgs e)
        {
            MessageBox.Show("Invalid lépés!");
        }

        private async void ViewModel_SaveGameAsync(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialógablak
                saveFileDialog.Title = "Kaméleonok betöltése";
                saveFileDialog.Filter = "Kaméleon tábla|*.txt";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (ChameleonDataException)
                    {
                        MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("A fájl mentése sikertelen!", "Sudoku", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ViewModel_LoadGameAsync(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Kaméleonok betöltése";
                openFileDialog.Filter = "Cameleon|*.txt";
                if (openFileDialog.ShowDialog() == true)
                {
                    await _model.LoadGameAsync(openFileDialog.FileName);
                }
            }
            catch (ChameleonDataException)
            {
                MessageBox.Show("A fájl betöltése sikertelen!", "CAmeleon", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewModel_NewGame(object sender, int size)
        {
            _model.NewGame(size);
        }

        private void Model_GameOver(object sender, ChameleonEventArgs e)
        {
            string winner = (e.WinnerChameleon == 1) ? "Piros" : "Zöld";
            MessageBox.Show("A játéknak vége!\n Nyertes: " + winner + " kaméleon!","Kaméleon",MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            _model.NewGame(3);
        }
    }
}
