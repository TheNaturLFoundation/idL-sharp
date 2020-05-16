namespace IDL_for_NaturL
{
    public static class LevenshteinUtils
    {
        public static int Min(int a, int b, int c)
        {
            if (a <= b && a <= c)
            {
                return a;
            }

            if (b <= a && b <= c)
                return b;
            return c;
        }

        public static int Levenshtein(string str1, string str2)
        {
            int cost = 0;
            int str1Length = str1.Length;
            int str2Length = str2.Length;
            int[,] matrice = new int [str1Length+1, str2Length+1];

            for (int i = 0; i <= str1Length; i++) matrice[i, 0] = i;

            for (int j = 0; j <= str2Length; j++) matrice[0, j] = j;

            for (int j = 1; j <= str2Length; j++)
            {
                for (int i = 1; i <= str1Length; i++)
                {
                    cost = str1[i-1] == str2[j-1] ? 0 : 1;
                    matrice[i, j] = Min(matrice[i - 1, j] + 1,
                        matrice[i, j - 1] + 1, matrice[i - 1, j - 1] + cost);
                }
            }

            return matrice[str1Length, str2Length];
        }
    }
}