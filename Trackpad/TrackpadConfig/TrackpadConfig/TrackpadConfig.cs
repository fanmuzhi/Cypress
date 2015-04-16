using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing;
using System.IO; 

namespace CypressSemiconductor.ChinaManufacturingTest
{
    [XmlRootAttribute("TrackpadConfig", Namespace = "", IsNullable = false)]
    public class TrackpadConfig
    {
        private Bitmap picture;
        
        /// <summary>
        /// Default constructor for this class (required for serialization).
        /// </summary>
        public TrackpadConfig()
        {
        }

        //Basic Information
        public double VDD_OP_PS;
        public double VDD_OP_IO;

        public double IDD_MAX;
        public double IDD_MIN;
        public double IDD_SHORT;
        public double IDD_OPEN;

        public byte I2C_ADDRESS;
        public byte I2C_SPEED;
        public string IDACMap;
        public int TRACKPAD_PLATFORM;
        public bool TRACKPAD_BOOTLOADER;
        public int TRACKPAD_TACTILESWITCH;
        public int TRACKPAD_INTERFACE;
        public int SERIALNUMBER_LENGTH;

        public int RESOLUTION_X;
        public int RESOLUTION_Y;

        [XmlIgnoreAttribute()]
        public Bitmap Picture
        {
            get { return picture; }
            set { picture = value; }
        }

        // Serializes the 'Picture' Bitmap to XML.
        [XmlElementAttribute("Picture")]
        public byte[] PictureByteArray
        {
            get
            {
                if (picture != null)
                {
                    TypeConverter BitmapConverter = TypeDescriptor.GetConverter(picture.GetType());
                    return (byte[])BitmapConverter.ConvertTo(picture, typeof(byte[]));
                }
                else
                    return null;
            }

            set
            {
                if (value != null)
                    picture = new Bitmap(new MemoryStream(value));
                else
                    picture = null;
            }
        }

        public testItems TPT = new testItems();
        public testItems TMT = new testItems();
        public testItems OQC = new testItems();

    }

    public class testItems
    {
        //FirmwareVersion
        [XmlAttribute]
        public bool ReadFW;
        public byte FW_INFO_FW_REV;
        public byte FW_INFO_FW_VER;
        public byte FW_INFO_NUM_COLS;
        public byte FW_INFO_NUM_ROWS;

        //Switch Test
        [XmlAttribute]
        public bool SwitchTest;

        //RawCount
        [XmlAttribute]
        public bool ReadRawCount;
        public int RAW_DATA_READS;
        public int RAW_AVG_MAX;
        public int RAW_AVG_MIN;
        public int RAW_STD_DEV_MAX;

        //Noise
        public int RAW_NOISE_MAX;

        //IDAC
        [XmlAttribute]
        public bool ReadIDAC;
        public byte IDAC_MAX;
        public byte IDAC_MIN;

        //IDAC Gain
        public byte IDAC_GAIN_MAX;

        //Local IDAC
        public byte LOCAL_IDAC_MIN;
        public byte LOCAL_IDAC_MAX;

        [XmlAttribute]
        public bool EraseIDAC;
    }

}
