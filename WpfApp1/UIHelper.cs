using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class UIHelper
    {
        private static readonly Lazy<UIHelper> _lazy = new Lazy<UIHelper>(() => new UIHelper());
        public static UIHelper Instance => _lazy.Value;
        public ASRockFanController FanController;
        public AsRockLedController LedController;
        public UIHelper()
        {
            FanController = new ASRockFanController();
            LedController = new AsRockLedController();
        }

        public void Dispose()
        {
            //FanController.Dispose();
            //LedController.Dispose();
        }




    }
}
