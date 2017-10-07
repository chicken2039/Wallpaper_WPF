using St2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// MorphForm.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MorphForm : Window
    {
        public MorphForm()
        {
            InitializeComponent();
        }
        public Renderer renderer;
        private string m_gifpath = "";
        public string ImagePath { get { return m_gifpath; } set { m_gifpath = value; } }
        //################################################//
        private string text_First;
        public string Text_First { get => text_First; set => text_First = value; }
        //################################################//
        private string text_Second;
        public string Text_Second { get => text_Second; set => text_Second = value; }
        //################################################//
        private bool m_isFixed = false;
        public bool IsFixed
        { get { return m_isFixed; } }
        //################################################//
    }
}
