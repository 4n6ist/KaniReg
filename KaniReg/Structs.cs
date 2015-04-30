namespace KaniReg {
	public struct Arguments
	{

		private string _hiveFileName;
		private string _hiveName;
		private short _timeZoneBias;
		private bool _outputUtc;

		public string HiveFileName
		{
			get
			{
				return _hiveFileName;
			}
		}

		public string HiveName
		{
			get
			{
				return _hiveName;
			}
		}

		public short TimeZoneBias
		{
			get
			{
				return _timeZoneBias;
			}
		}

		public bool OutputUtc
		{
			get
			{
				return _outputUtc;
			}
		}

		public Arguments(string hiveFileName, string hiveName, short TimeZoneBias, bool outputUtc)
		{
			_hiveFileName = hiveFileName;
			_hiveName = hiveName;
			_timeZoneBias = TimeZoneBias;
			_outputUtc = outputUtc;
		}
	}

    public struct Container {
        private string _name;
        private string _data;

        public Container(string name, string data) {
            _name = name;
            _data = data;
        }

        public string Data {
            set { _data = value; }
            get { return _data; }
        }

        public string Name {
            set { _name = value; }
            get { return _name; }
        }
    }

    public struct TimestampContainer {
        private double _timestamp;
        private string _data;

        public TimestampContainer(double timestamp, string data) {
            _timestamp = timestamp;
            _data = data;
        }

        public double Timestamp {
            set { _timestamp = value; }
            get { return _timestamp; }
        }

        public string Data {
            set { _data = value; }
            get { return _data; }
        }
    }

    public struct TimestampContainer2 {
        private string _name;
        private double _timestamp;

        public TimestampContainer2(string name, double timestamp) {
            _name = name;
            _timestamp = timestamp;
        }

        public string Name {
            set { _name = value; }
            get { return _name; }
        }

        public double Timestamp {
            set { _timestamp = value; }
            get { return _timestamp; }
        }
    }

    public struct UshortContainer {

        private ushort _order;
        private string _data;

        public UshortContainer(ushort order, string data) {
            _order = order;
            _data = data;
        }

        public ushort Order {
            set { _order = value; }
            get { return _order; }
        }

        public string Data {
            set { _data = value; }
            get { return _data; }
        }
    }

    public struct CombinedContainer {
        private string _name;
        private string _data;
        private double _timestamp;

        public CombinedContainer(string name, string data, double timestamp) {
            _name = name;
            _data = data;
            _timestamp = timestamp;
        }

        public string Name {
            set { _name = value; }
            get { return _name; }
        }

        public string Data {
            set { _data = value; }
            get { return _data; }
        }

        public double Timestamp {
            set { _timestamp = value; }
            get { return _timestamp; }
        }
    }

    public struct ComplexContainer {
        private string _name;
        private string _displayName;
        private string _imagePath;
        private string _objectName;

        public ComplexContainer(string name, string displayName, string imagePath, string objectName) {
            _name = name;
            _displayName = displayName;
            _imagePath = imagePath;
            _objectName = objectName;
        }

        public string Name {
            set { _name = value; }
            get { return _name; }
        }

        public string DisplayName {
            set { _displayName = value; }
            get { return _displayName; }
        }

        public string ImagePath {
            set { _imagePath = value; }
            get { return _imagePath; }
        }

        public string ObjectName {
            set { _objectName = value; }
            get { return _objectName; }
        }
    }
}
