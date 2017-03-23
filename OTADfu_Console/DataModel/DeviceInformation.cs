﻿/*Copyright (c) 2015, Nordic Semiconductor ASA
 *
 *Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 *
 *1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 *
 *2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other 
 *materials provided with the distribution.
 *
 *3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific 
 *prior written permission.
 *
 *THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 *PURPOSE ARE DISCLAIMED. *IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF *SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, *DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 *ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED *OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage;
using SDKTemplate;

namespace DataModel
{
    public class DeviceInformationItem
    {
        public GattDeviceService Device { get; set; }
        public string DeviceAddress { get; set; }
        public List<Boolean> Flags { get; set; }
        public string UUID { get; set; }
        public string CompleteDeviceName { get; set; }
        public bool AlertOnPhone { get; set; }
        public bool AlertOnDevice { get; set; }
        public string DeviceID { get; set; }
        public Dictionary<string, GattDeviceService> Services = new Dictionary<string, GattDeviceService>();
        private BLEDeviceConnectionStatusHandler _connectionStatus;
        public BLEDeviceConnectionStatusHandler ConnectionStatus
        {
            get
            {
                return _connectionStatus;
            }
            set
            {

                this._connectionStatus = value;
            }
        }

        public delegate void BLEDeviceConnectionStatusHandler(bool isConnected);

        private static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        
        public DeviceInformationItem(string deviceAddress, List<Boolean> flags, string uUID, string completeName)
            
        {
            this.DeviceAddress = deviceAddress;
            this.Flags = flags;
            this.UUID = uUID;
            this.CompleteDeviceName = completeName;
        }
    }
}
