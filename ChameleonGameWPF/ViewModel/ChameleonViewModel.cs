using ChameleonGameWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ChameleonGameWPF.ViewModel
{
    class ChameleonViewModel : ViewModelBase
    {
        private ChameleonModel _model;
        private ChameleonField _prev;
        private string _imgPath;

        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand NewGameCommand { get; private set; }
        public ObservableCollection<ChameleonField> Fields { get; set; }
        public string CurrentChameleon{get => _model.CurrentChameleon;}

        public event EventHandler LoadGame;
        public event EventHandler SaveGame;
        public event EventHandler<int> NewGame;
        public event EventHandler InvalidStep;

        public ChameleonViewModel(ChameleonModel model)
        {
            _prev = null;
            Fields = new ObservableCollection<ChameleonField>();
            _imgPath = "/ChameleonGameWPF;Component/Icons/";
            _model = model;

            _model.TableRefresh += new EventHandler(Model_TableRefresh);
            _model.GameCreated += new EventHandler(Model_GameCreated);

            NewGameCommand = new DelegateCommand(param => OnNewGame(Convert.ToInt32(param)));
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            
        }

        private void Model_TableRefresh(object sender, EventArgs e)
        {
            RefreshTable();
        }

        private void Model_GameCreated(object sender, EventArgs e)
        {
            Fields.Clear();
            for (Int32 i = 0; i < _model.Table.Size; i++)
            {
                for (Int32 j = 0; j < _model.Table.Size; j++)
                {
                    Fields.Add(new ChameleonField
                    {
                        Color = _model.Table.GetColor(i, j),
                        Chameleon = _model.Table[i, j],
                        Source = (_model.Table[i, j] == 1) ? _imgPath + "red.png" : ((_model.Table[i, j] == 2) ? _imgPath + "green.png" : ""),
                        X = i,
                        Y = j,
                        IsEnable = true,
                        Number = i * _model.Table.Size + j,
                        StepCommand = new DelegateCommand(param => StepGame(Convert.ToInt32(param)))
                    });
                }
            }
            OnPropertyChanged("CurrentChameleon");
        }

        private void StepGame(int v)
        {
            ChameleonField _current = Fields[v];

            if(_prev is null)
            {
                _prev = _current;
                if (_model.Table[_prev.X, _prev.Y] == 0)
                    _prev= null;
                return;
            }

            int x1 = _prev.X;
            int y1 = _prev.Y;
            int x2 = _current.X;
            int y2 = _current.Y;
            if (!_model.Step(x1, y1, x2, y2))
            {
                OnInvalidStep();
            }
            _prev = null;
            OnPropertyChanged("CurrentChameleon");
        }

        private void RefreshTable()
        {
            foreach(var field in Fields)
            {
                field.Chameleon = _model.Table[field.X,field.Y];
                field.Source = (_model.Table[field.X, field.Y] == 1) ? _imgPath + "red.png" : ((_model.Table[field.X, field.Y] == 2) ? _imgPath + "green.png" : "");
            }
        }
        private void OnNewGame(int size)
        {
            if (NewGame != null)
                NewGame(this,size);
        }

        private void OnSaveGame()
        {
            if (SaveGame != null)
                SaveGame(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            if (LoadGame != null)
                LoadGame(this, EventArgs.Empty);
        }

        private void OnInvalidStep()
        {
            if (InvalidStep != null)
                InvalidStep(this, EventArgs.Empty);
        }
    }
}
