using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   //Global variables
        int A;  //Age of client
        int H;  //Height of client
        int W;  //Weight of client
        double BMI;
        bool otvoren = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                A = Int32.Parse(AgeTxt.Text);
                H = Int32.Parse(HeightTxt.Text);
                W = Int32.Parse(WeightTxt.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("You didn't enter all *required  informations.", "Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
                
            }



            //BMI DISPLAY
            BMI = Math.Round(((W / Math.Pow(H, 2)) * 10000),2);
            BmiTxt.Text = BMI.ToString();

            //BODY FAT DISPLAY
            double bodyfat = BodyFat(A);
            FatTxt.Text = bodyfat.ToString();

            //BMR DISPLAY
            double bmr=Final_BMR_Average();
            BMRResult.Text = bmr.ToString();

            //TDEE DISPLAY
            double tdee = Goals();
            TDEEResult.Text = tdee.ToString();

        }
        //*************************************BMR Methodology (Formula)****************************************

        //Harris Benedict (original) calculation 
        private int Harris_Benedict_Original(int a, int h, int w)
        {
            double res = 0;
            if (MaleBox.IsChecked == true)
                res = (13.7516 * w) + (5.0033 * h) - (6.755 * a) + 66.473;
            if (FemaleBox.IsChecked == true)
                res = (9.5634 * w) + (1.8496 * h) - (4.6756 * a) + 655.0955;
            
            return (int)res;
        }

        //Harris Benedict (revised) calculation
        private int Harris_Benedict_Revised(int a, int h, int w)
        {
            double res = 0;
            if (MaleBox.IsChecked == true)
                res = (13.397 * w) + (4.799 * h) - (5.677 * a) + 88.362;
            if (FemaleBox.IsChecked == true)
                res = (9.247 * w) + (3.098 * h) - (4.330 * a) + 447.593;
            return (int)res;
        }
        //Mifflin St Jeor 
        private int Mifflin_St_Jeor(int a, int h, int w)
        {
            double res=0;
            if(MaleBox.IsChecked==true)
                res = (10 * w) + (6.25 * h) - (5 * a) + 5;
            if(FemaleBox.IsChecked==true)
                res = (10 * w) + (6.25 * h) - (5 * a) -161;
            return (int)res;
        }
        //Katch-McArdle
        private int Katch_McArdle( int a,int w)
        {
            double res;
            double lbm= (w * (1 - BodyFat(a)/100));
            res = 370 + (21.6 *lbm);
            return (int)res;
        }
        //LBM is lean body mass
        //Katch-McArdle Hybrid
        private int Katch_McArdle_Hybrid(int a, int w)
        {
            double res;
            double lbm = (w * (1 - (BodyFat(a)) / 100));
            res = 370 + (21.6 * lbm);
            return (int)res;
        }
        //Cunningham
        private int Cunningham(int a, int w)
        {

            
            double lbm = (w * (100 - (BodyFat(a))) / 100);
            double res;
            res = 500 + (22 * lbm);
            return (int)res;
        }
        
        //Final (Average)
        private int Final_BMR_Average()
        {
            double BMR;
            int HarrisOrigin = Harris_Benedict_Original(A,H,W);
            int HarrisRevised=Harris_Benedict_Revised(A,H,W);
            int Mifflin = Mifflin_St_Jeor(A,H,W);
            int McArdle = Katch_McArdle(A,W);
            int McArdleHybrid = Katch_McArdle_Hybrid(A,W);
            int Cunning= Cunningham(A,W);
            BMR = (HarrisOrigin + HarrisRevised + Mifflin + McArdle + McArdleHybrid + Cunning) / 6;
            return (int)BMR;
        }
        //**************************************************************************************************

        private double BodyFat(int a)
        {
            double fprecent = 0;
            if (MaleBox.IsChecked == true)
                fprecent = 1.20 * BMI + 0.23 * a - 10.8 * 1 - 9;
            else if (MaleBox.IsChecked == false)
                fprecent = 1.20 * BMI + 0.23 * a - 10.8 * 0 - 9;
            else
            {
                MessageBox.Show("Please enter the gender.",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            FatTxt.Text = fprecent.ToString();
            return fprecent;
        }
        private double Get_TDEE()
        {
            double bmr = Final_BMR_Average();
            double tdee=0;
            switch (ActivityCombo.SelectedIndex)
            {
                case 0:
                    tdee = bmr * 1.20;
                    break;
                case 1:
                    tdee = bmr * 1.40;
                    break;
                case 2:
                    tdee = bmr * 1.55;
                    break;
                case 3:
                    tdee = bmr * 1.75;
                    break;

                default:
                    MessageBox.Show("Chose activity level if you want to measure your TDEE.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
            return tdee;
        }
        private double Goals()
        {
            double tdee = Get_TDEE();
            double goal = 0;
            if (LoseBox.IsChecked == true && MaintainBox.IsChecked == false && GainBox.IsChecked == false)
                goal = tdee - 500;
            else if (LoseBox.IsChecked == false && MaintainBox.IsChecked == true && GainBox.IsChecked == false)
                goal = tdee;
            else if (LoseBox.IsChecked == false && MaintainBox.IsChecked == false && GainBox.IsChecked == true)
                goal = tdee + 500;
            else
                goal = tdee;
            return goal;

        }
        private void MaleBox_Checked(object sender, RoutedEventArgs e)
        {
            FemaleBox.IsChecked = false;
        }

        private void FemaleBox_Checked(object sender, RoutedEventArgs e)
        {
            MaleBox.IsChecked = false;
        }

        private void LoseBox_Checked(object sender, RoutedEventArgs e)
        {
            MaintainBox.IsChecked = false;
            GainBox.IsChecked = false;
        }

        private void MaintainBox_Checked(object sender, RoutedEventArgs e)
        {
            LoseBox.IsChecked = false;
            GainBox.IsChecked = false;
        }

        private void GainBox_Checked(object sender, RoutedEventArgs e)
        {
            LoseBox.IsChecked = false;
            MaintainBox.IsChecked = false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            AgeTxt.Text = "";
            HeightTxt.Text = "";
            WeightTxt.Text = "";
            MaleBox.IsChecked = false;
            FemaleBox.IsChecked = false;
            FatTxt.Text = "";
            BmiTxt.Text = "";
            LoseBox.IsChecked = false;
            MaintainBox.IsChecked = false;
            GainBox.IsChecked = false;
            ActivityCombo.SelectedValue = false;
            BMRResult.Text = "";
            TDEEResult.Text = "";
        }
        


        //*************************************INFO BOXES*************************************
        private void BodyFatInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BODYFAT caption = new BODYFAT();
            caption.ShowDialog();
        }

        private void BMRInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BMR caption = new BMR();
            caption.ShowDialog();
        }

        private void BMIInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BMI caption = new BMI();
            caption.ShowDialog();
        }

        private void TDEEInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TDEE caption = new TDEE();
            // caption.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            caption.ShowDialog();
        }
        //*****************************************************************************************

    }
}
