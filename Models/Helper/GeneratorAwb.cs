namespace PacheteAPP.Models.Helper;
public static class GeneratorAwb
{
    public static string GenerareAwb(long id)
    {
        char[] map = { 'E', 'A', 'R', 'I', 'O', 'T', 'N', 'S', 'X', 'Z' };
        string idString = id.ToString();

        string mappedIdPart = "";
        foreach (char c in idString)
        {
            int digit = int.Parse(c.ToString());
            mappedIdPart += map[digit];
        }

        int targetLength = 10;
        string charsPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random random = new Random();

        string randomPart = "";
        int remainingLength = targetLength - mappedIdPart.Length;

        if (remainingLength > 0)
        {
            for (int i = 0; i < remainingLength; i++)
            {
                randomPart += charsPool[random.Next(charsPool.Length)];
            }
        }
        else
        {
            mappedIdPart = mappedIdPart.Substring(0, targetLength);
        }

        string uniqueCode = mappedIdPart + randomPart;
        string datePart = DateTime.Now.ToString("yyyyMMdd");

        return $"AWB-{datePart}-{uniqueCode}";
    }
}