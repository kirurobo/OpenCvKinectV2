using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Kinect;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace OpenCvKinectV2
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// KinectSensorのインスタンス
        /// </summary>
        KinectSensor kinect;

        /// <summary>
        /// 最新のカラー画像
        /// </summary>
        Mat colorImage;

        /// <summary>
        /// 出力用に処理した画像
        /// </summary>
        Mat colorOutputImage;

        /// <summary>
        /// PictureBoxで表示するビットマップ
        /// </summary>
        Bitmap colorBitmap;


        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ウィンドウが開いたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // KinectSensorのインスタンスを取得
            this.kinect = KinectSensor.GetDefault();

            // カラー、深度等をまとめて取得するイベントハンドラを設定
            MultiSourceFrameReader multiSourceFrameReader = this.kinect.OpenMultiSourceFrameReader(
                FrameSourceTypes.Color      // 今回はカラーのみ
                );
            multiSourceFrameReader.MultiSourceFrameArrived += FrameArrived;

            // 画像の準備
            InitializeImage();

            // Kinect利用開始
            this.kinect.Open();
        }

        /// <summary>
        /// ウィンドウが閉じられたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Kinect利用終了
            this.kinect.Close();
        }

        /// <summary>
        /// 画像を新規作成
        /// </summary>
        private void InitializeImage()
        {
            // カラー画像の準備
            FrameDescription colorFrameDescription = this.kinect.ColorFrameSource.FrameDescription;
            this.colorImage = new Mat(
                colorFrameDescription.Height,
                colorFrameDescription.Width,
                MatType.CV_8UC4                     // 8ビット×4チャンネル分
                );
            this.colorImage.SetTo(Scalar.All(255)); // 画像全体を白色に塗りつぶし

            // 同じサイズ・深度で出力用画像も準備
            this.colorOutputImage = this.colorImage.Clone();

            // カラー画像Bitmapの作成とPictureBoxへの割り当て
            this.colorBitmap = this.colorOutputImage.ToBitmap();
            this.pictureBoxColorFrame.Image = this.colorBitmap;
        }

        /// <summary>
        /// Kinectのデータ取得時に呼ばれる処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();

            // カラー画像取得時の処理
            using (ColorFrame colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    // OpenCVの画像にKinectのカラー画像を複製
                    colorFrame.CopyConvertedFrameDataToIntPtr(
                        this.colorImage.Data,   // カラー画像Matのデータ部分ポインタ
                        (uint)(this.colorImage.Total() * this.colorImage.ElemSize()),   // 全画素数 × 一画素のバイト数
                        ColorImageFormat.Bgra   // RGBでなくBGRAの順番
                        );

                    // カラー画像からPictureBoxへの描画
                    DrawColorImage();
                }
            }
        }

        /// <summary>
        /// PictureBoxに表示させる
        /// </summary>
        private void DrawColorImage()
        {
            // RGBそれぞれでの閾値処理
            Cv2.Threshold(this.colorImage, this.colorOutputImage, 127.0, 255.0, ThresholdTypes.Binary);
            //// 次のような書き方もできるが、メモリを消費してしまう
            //this.colorOutputImage = this.colorImage.Threshold(127.0, 255.0, ThresholdTypes.Binary);

            // 画像からビットマップに上書き
            this.colorOutputImage.ToBitmap(this.colorBitmap);

            // PictureBoxの描画を要求
            this.pictureBoxColorFrame.Invalidate();
        }
    }
}
