using TEKSteamClient;
namespace DepotDecryptionKeys
{
    internal class Program
    {
        public static byte[] ConvertHexStringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length.");

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
        public static bool IsHexStringValid(string hex)
        {
            // Kiểm tra xem chuỗi có chứa các ký tự hợp lệ hay không
            return hex.All(c => "0123456789abcdefABCDEF".Contains(c));
        }
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Vui lòng cung cấp các tham số: -depotId <ID> -hexString <chuỗi hex>");
                return;
            }
            int depotId = 0;
            string hexString = string.Empty;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-depotId" && i + 1 < args.Length)
                {
                    depotId = int.Parse(args[i + 1]);
                }
                else if (args[i] == "-hexString" && i + 1 < args.Length)
                {
                    hexString = args[i + 1];
                }
            }
            if (depotId == 0 || string.IsNullOrEmpty(hexString))
            {
                Console.WriteLine("Tham số không hợp lệ. Vui lòng kiểm tra lại.");
                return;
            }

            if (!hexString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hexString = "0x" + hexString;
            }
            AddDepotDecryptionKey((uint)depotId, hexString);

            Console.WriteLine("Depot Decryption Key for Depot ID " + depotId + ":");
            Console.WriteLine(string.Join(", ", CDNClient.DepotDecryptionKeys[(uint)depotId].Select(b => "0x" + b.ToString("X2"))));
        }
        public static void AddDepotDecryptionKey(uint depotId, string hexString)
        {
            // Bỏ qua "0x" khi chuyển đổi
            string hexWithoutPrefix = hexString.Substring(2);

            // Kiểm tra tính hợp lệ của chuỗi hex
            if (!IsHexStringValid(hexWithoutPrefix))
            {
                Console.WriteLine("Chuỗi hex không hợp lệ.");
                return;
            }
            byte[] decryptionKey = ConvertHexStringToByteArray(hexWithoutPrefix);
            CDNClient.DepotDecryptionKeys[depotId] = decryptionKey; // Thêm hoặc cập nhật khóa giải mã cho depot
        }

    }
}
