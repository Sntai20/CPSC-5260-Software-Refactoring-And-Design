static int[] InitializeCombination(int length)
{
    int[] combination = new int[length];
    for (int i = 0; i < length; i++)
    {
        combination[i] = i + 1;
    }

    return combination;
}

static void PrintCombination(int[] combination)
{
    Console.WriteLine(string.Join(" ", combination));
}

static bool NextCombination(int[] combination, int range)
{
    int index = combination.Length - 1;
    while (index >= 0 && combination[index] == range - (combination.Length - 1 - index))
    {
        index--;
    }

    if (index < 0)
    {
        return false;
    }

    combination[index]++;
    for (int i = index + 1; i < combination.Length; i++)
    {
        combination[i] = combination[i - 1] + 1;
    }

    return true;
}

int range = 20;
int totalCombinations = 0;
int[] combination = InitializeCombination(6);

do
{
    PrintCombination(combination);
    totalCombinations++;
}
while (NextCombination(combination, range));

Console.WriteLine("TotalCombinations: " + totalCombinations);