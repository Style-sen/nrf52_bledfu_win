﻿using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using System;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Security.Permissions;
using System.Security;
using System.Threading;

namespace OTADFUApplication
{    
    /// <summary>
    /// The main program
    /// </summary>
    public class Program
    {  
        //TODO
        public String path = @"E:\nrf52_bledfu_win_console";
        StreamWriter logFile = null;
        //String logPath = @"C:\logs\";
        String logPath = @".\logs\";

        Program(){
            String time = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            String logfilename = "[" + time + "].txt";
            
            Console.WriteLine("Log filename:" + logfilename);
            try
            {
                Directory.CreateDirectory(logPath);
                logFile = new StreamWriter(logPath + logfilename, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">
        /// 0: command (help, scan or update)
        /// 1: file type (-f for bin file, -z for zip file)
        /// 2: the path of bin file (if args[1] is -f) or the zip file (if args[1] is -z)
        /// 3: -d  (if args[1] is -f) or -a (if args[1] is -z)
        /// 4: the dat file path (if args[1] is -f) or the device address (if args[1] is -z)
        /// 5: -a (if args[1] is -f) 
        /// 6: the device address (if args[1] is -f)
        /// </param>
        [STAThread]
        static void Main(String[] args)
        {
            //if (args.Length  > 0) //debug
            if (args.Length == 0)
                Console.WriteLine("Usage: otadfu [help] or [scan] or [update -f <bin_file> -d <dat_file> -a <device_address>] or [update -z <zip_file> -a <device_address>]");
            else if (args[0] == "help")
            {
                Console.WriteLine("OTA DFU update application for nrf5x MCUs. Visit https://github.com/astronomer80/nrf52_bledfu_win for more information");
                Console.WriteLine("Here a list of commands available:");
                Console.WriteLine("help: Show this help");
                Console.WriteLine("scan: Scan BLE devices already paired with Windows Settings");
                Console.WriteLine("update -f < bin_file > -d < dat_file > -a <device_address>. bin_file is the file generated from the Arduino IDE. dat_file is the init packet generated by nrfutil application. device_address: is the address of the device returned using 'scan' command.");
                Console.WriteLine("update -z < zip_file > -a <device_address>. zip_file is the archive generated by nrfutil application. device_address: is the address of the device returned using 'scan' command.");
            }
            //Scan only paired BLE devices
            else if (args[0] == "scan") {
                new Program().MainTask(true, "", "", "");
            }
            //Update procedure
            else if (args[0] == "update")
            {
                //Update from decompressed files
                if (args.Length >= 7 && args[1] == "-f" && args[3] == "-d" && args[5] == "-a")
                    new Program().MainTask(false, args[2], args[4], args[6]);
                //Update from zipped package
                else if (args.Length >= 5 && args[1] == "-z" && args[3] == "-a") {
                    String tmp = Directory.GetCurrentDirectory();// .GetDirectoryRoot(args[2]);
                    Console.WriteLine(tmp);
                    ZipFile.ExtractToDirectory(args[2], ".");
                    String bin_file = args[2].Replace(".zip", ".bin");
                    String dat_file = args[2].Replace(".zip", ".dat");
                    File.Delete(bin_file);
                    File.Delete(dat_file);
                    new Program().MainTask(false, bin_file, dat_file, args[4]);
                }else
                    Console.WriteLine("Invalid update command. Type 'otadfu help' for more information");
            }
            else {
                Console.WriteLine("Unknown command. Type 'otadfu help' for more information");
                    
            }
            Console.WriteLine("Press a key to close");
            Console.ReadLine();
        }
        
        /// <summary>
        /// Write log data 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="appendname"></param>
        public void log(string data, string tag)
        {
            Console.WriteLine(data);
            try
            {
                if (tag.Equals(""))
                    this.logFile.WriteLine("[" + tag + "]" + data);
                else
                    this.logFile.WriteLine(data);
                this.logFile.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Defines the Main asynchronous task
        /// </summary>
        /// <returns></returns>
        public async Task MainTask(bool scanonly, String bin_file, String dat_file, String device_address)
        {
            this.log("MainTask", "");
            try
            {
                await readevices(scanonly, bin_file, dat_file, device_address);                
            }
            catch (Exception e)
            {
                this.log(e.StackTrace, "Test");
            }
        }

        /// <summary>
        /// Read the list of paired devices
        /// </summary>
        /// <param name="scanonly">If true return only the list of paired devices. 
        /// If false starts the OTA for the device address defined at the argument of Main</param>
        /// <returns></returns>
        private async Task readevices(bool scanonly, String bin_file, String dat_file, String given_device_address)
        {
            this.log("Scanning BLE devices...", "");
            Guid UUID = new Guid(DFUService.DFUService_UUID); //NRF52 DFU Service
            //Guid UUID = new Guid("00001530-1212-efde-1523-785feabcd123"); //NRF52 DFU Service            
            String service = GattDeviceService.GetDeviceSelectorFromUuid(UUID);
            String[] param = new string[] { "System.Devices.ContainerId" };         
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(service, param);
            Thread.Sleep(2000);  //TODO Remove this delay
            if (devices.Count > 0)
            {
                foreach (DeviceInformation device in devices)
                {
                    var deviceAddress = "not available";
                    //Console.WriteLine(device.Name + " " + device.Id);                    
                    //Parse device address
                    if (device.Id.Contains("_") && device.Id.Contains("#"))
                        deviceAddress = device.Id.Split('_')[1].Split('#')[0];
                    Console.WriteLine(device.Name + " " + deviceAddress);
                    //foreach (var prop in device.Properties) {
                    //    Console.WriteLine(prop.Key + " " + prop.Value);                        
                    //}
                    //TODO
                    //if(!scanonly && given_device_address==deviceAddress)
                    if (!scanonly && true) {
                        try
                        {
                            //DFUService dfs =DFUService.Instance;
                            //await dfs.InitializeServiceAsync(device);
                            await DFUService.Instance.InitializeServiceAsync(device, this, bin_file, dat_file);
                        }
                        catch (Exception e) {
                            Console.WriteLine(e);
                        }
                        
                    }
                    
                }
            }
            else
            {
                this.log("No paired BLE devices found", "Test");
            }
            

        }
    }
}