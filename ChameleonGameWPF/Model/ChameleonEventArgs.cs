using ChameleonGameWPF.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChameleonGameWPF.Model
{
    public class ChameleonEventArgs
    {
        private int _winnerChameleon;

        public ChameleonEventArgs(int winnerChameleon)
        {
            WinnerChameleon = winnerChameleon;
        }

        public int WinnerChameleon { get => _winnerChameleon; private set => _winnerChameleon = value; }
    }
}
