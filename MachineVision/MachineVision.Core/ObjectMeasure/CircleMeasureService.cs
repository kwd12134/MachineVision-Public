﻿using HalconDotNet;
using MachineVision.Core.Extensions;
using MachineVision.Core.TemplateMatch.Shared;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision.Core.ObjectMeasure
{
    public class CircleMeasureService : BindableBase
    {
        /// <summary>
        /// 找圆服务
        /// </summary>
        public CircleMeasureService()
        {
            Info = new MethodInfo()
            {
                Name = "find_circle",
                Description = "Add a circle or a circular arc to a metrology model.",
                Parameters = new List<MethodParameter>()
                {
                   new MethodParameter(){ Name="MetrologyHandle", Description="Handle of the metrology model." },
                   new MethodParameter(){ Name="Row ", Description="Row coordinate (or Y) of the center of the circle or circular arc." },
                   new MethodParameter(){ Name="Column ", Description="Column (or X) coordinate of the center of the circle or circular arc." },
                   new MethodParameter(){ Name="Radius", Description="Radius of the circle or circular arc." },
                   new MethodParameter(){ Name="MeasureLength1 ", Description="Half length of the measure regions perpendicular to the boundary." },
                   new MethodParameter(){ Name="MeasureLength2", Description="Half length of the measure regions tangetial to the boundary." },
                   new MethodParameter(){ Name="MeasureSigma ", Description="Sigma of the Gaussian function for the smoothing." },
                   new MethodParameter(){ Name="MeasureThreshold", Description="Minimum edge amplitude." },
                   new MethodParameter(){ Name="GenParamName ", Description="Names of the generic parameters." },
                   new MethodParameter(){ Name="GenParamValue ", Description="Values of the generic parameters." },
                   new MethodParameter(){ Name="Index ", Description="Index of the created metrology object." },
                },
                Predecessors = new List<string>()
                {
                     "align_metrology_model",
                     "apply_metrology_model",
                }
            };
            RunParameter = new MeasureRunParameter();
            RunParameter.ApplyDefaultParameter();
        }
        private HWindow hWindow;

        public HWindow HWindow
        {
            get { return hWindow; }
            set { hWindow = value; RaisePropertyChanged(); }
        }

        public MethodInfo Info { get; set; }
        public RoiParameter Roi { get; set; }

        private MeasureRunParameter runParameter;

        HObject ho_Contour, ho_Contours;
        HTuple hv_MetrologyHandle = new HTuple(), hv_Index = new HTuple(), hv_Row = new HTuple();
        HTuple hv_Parameter = new HTuple();
        HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();

        /// <summary>
        /// 查找圆运行参数
        /// </summary>
        public MeasureRunParameter RunParameter
        {
            get { return runParameter; }
            set { runParameter = value; RaisePropertyChanged(); }
        }
        public void Run(HObject Image)
        {
            if (Image == null) return;

            if (RunParameter.Radius == 0) return;
            if (RunParameter.Row == 0 || RunParameter.Column == 0) return;

            var ho_GrayImage = Image.RgbToGray();

            hv_MetrologyHandle.Dispose();
            HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);

            hv_Index.Dispose();
            HOperatorSet.AddMetrologyObjectCircleMeasure(hv_MetrologyHandle,
                RunParameter.Row,
                RunParameter.Column,
                RunParameter.Radius,
                RunParameter.MeasureLength1,
                RunParameter.MeasureLength2,
                RunParameter.MeasureSigma,
                RunParameter.MeasureThreshold,
                new HTuple(), new HTuple(), out hv_Index);
            //提取卡尺轮廓
            HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row1, out hv_Column1);
            HOperatorSet.ApplyMetrologyModel(ho_GrayImage, hv_MetrologyHandle);
            //hv_Parameter.Dispose();
            HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, 0, "all", "result_type", "all_param", out hv_Parameter);
            //ho_Contour.Dispose();
            HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, 0, "all", 1.5);
            if (HWindow != null)
            {
                HOperatorSet.SetColor(HWindow, "red");
                HWindow.DispObj(ho_Contours);
                HOperatorSet.SetColor(HWindow, "blue");
                HWindow.DispObj(ho_Contour);

                HWindow.SetString($"查找圆坐标:({hv_Parameter.DArr[0]},{hv_Parameter.DArr[1]})，半径:{hv_Parameter.DArr[2]}", "image", 10, 10, "black", "true");
                HOperatorSet.DispCross(HWindow, hv_Parameter.DArr[0], hv_Parameter.DArr[1], 50, 0);
            }

        }

    }
}
