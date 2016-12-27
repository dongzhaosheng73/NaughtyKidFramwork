using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Management;
using NaughtyKid.Error;


namespace NaughtyKid.DevicesControl
    {
        /// <summary>
        /// USB控制设备类型
        /// </summary>
        public struct UsbControllerDevice
        {
            /// <summary>
            /// USB控制器设备ID
            /// </summary>
            public String Antecedent;

            /// <summary>
            /// USB即插即用设备ID
            /// </summary>
            public String Dependent;
        }

        /// <summary>
        /// 监视USB插拔
        /// </summary>
        public partial class UsbDeviceHelper
        {
            /// <summary>
            /// USB插入事件监视
            /// </summary>
            private ManagementEventWatcher insertWatcher = null;

            /// <summary>
            /// USB拔出事件监视
            /// </summary>
            private ManagementEventWatcher removeWatcher = null;

            /// <summary>
            /// 添加USB事件监视器
            /// </summary>
            /// <param name="usbInsertHandler">USB插入事件处理器</param>
            /// <param name="usbRemoveHandler">USB拔出事件处理器</param>
            /// <param name="withinInterval">发送通知允许的滞后时间</param>
            public Boolean AddUSBEventWatcher(EventArrivedEventHandler usbInsertHandler, EventArrivedEventHandler usbRemoveHandler, TimeSpan withinInterval)
            {
                try
                {
                  
                    ManagementScope Scope = new ManagementScope("root\\CIMV2");
                    Scope.Options.EnablePrivileges = true;

                    // USB插入监视
                    if (usbInsertHandler != null)
                    {
                        WqlEventQuery InsertQuery = new WqlEventQuery("__InstanceCreationEvent",
                            withinInterval,
                            "TargetInstance isa 'Win32_USBControllerDevice'");

                        insertWatcher = new ManagementEventWatcher(Scope, InsertQuery);
                        insertWatcher.EventArrived += usbInsertHandler;
                        insertWatcher.Start();
                    }

                    // USB拔出监视
                    if (usbRemoveHandler != null)
                    {
                        WqlEventQuery RemoveQuery = new WqlEventQuery("__InstanceDeletionEvent",
                            withinInterval,
                            "TargetInstance isa 'Win32_USBControllerDevice'");

                        removeWatcher = new ManagementEventWatcher(Scope, RemoveQuery);
                        removeWatcher.EventArrived += usbRemoveHandler;
                        removeWatcher.Start();
                    }

                    return true;
                }

                catch (Exception ex)
                {

                    ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);

                    RemoveUSBEventWatcher();
                    return false;
                }
            }

            /// <summary>
            /// 移去USB事件监视器
            /// </summary>
            public void RemoveUSBEventWatcher()
            {
                try
                {
                    if (insertWatcher != null)
                    {
                        insertWatcher.Stop();
                        insertWatcher = null;
                    }

                    if (removeWatcher != null)
                    {
                        removeWatcher.Stop();
                        removeWatcher = null;
                    }
                }
                catch (Exception ex)
                {
                    ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                   
                }
               
            }

            /// <summary>
            /// 定位发生插拔的USB设备
            /// </summary>
            /// <param name="e">USB插拔事件参数</param>
            /// <returns>发生插拔现象的USB控制设备ID</returns>
            public static UsbControllerDevice[] WhoUSBControllerDevice(EventArrivedEventArgs e)
            {
                try
                {
                    var mbo = e.NewEvent["TargetInstance"] as ManagementBaseObject;
                    if (mbo != null && mbo.ClassPath.ClassName == "Win32_USBControllerDevice")
                    {
                        var Antecedent = ((string) mbo["Antecedent"]).Replace("\"", string.Empty).Split(new Char[] { '=' })[1];
                        var Dependent = ((string) mbo["Dependent"]).Replace("\"", string.Empty).Split(new Char[] { '=' })[1];

                        return new UsbControllerDevice[1] { new UsbControllerDevice { Antecedent = Antecedent, Dependent = Dependent } };
                    }
                    return null;
                }
                catch (Exception ex) 
                {
                    ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return null;
                }
               
            }
        }
    }

