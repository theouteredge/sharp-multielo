namespace theouteredge.mulielo.test
{
  public class ScoringFunctionsTests
  {
    Func<IEnumerable<double>, string> format = (scores) => $"[{string.Join(',', scores)}]";

    Func<IEnumerable<double>, List<double>> difference = (scores) =>
    {
      var result = new List<double>();
      var list = scores.ToList();
      for (var i = 0; i < list.Count - 1; i++)
      {
        result.Add(Math.Abs(list[i] - list[i + 1]));
      }

      return result;
    };

    Func<int, int, int, List<double>> random = (from, to, count) =>
    {
      var rnd = new Random();
      var results = new List<double>(count);

      for (var i = 0; i < count; i++)
        results.Add(rnd.Next(from, to) + rnd.NextDouble());

      return results;
    };


    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void Assert_That_Liner_Scoring_Works()
    {
      Enumerable.Range(3, 11)
      .ToList()
      .ForEach(n =>
      {
        var scoringMethod = Scoring.Create(1);
        var scores = scoringMethod(n);

        Assert.That(scores.Sum(), Is.EqualTo(1).Within(0.000000000000001),
          () => $"liner scoring algorithm does not sum to 1 for n={n}: {format(scores)}");

        Assert.That(scores.Min(), Is.EqualTo(0),
          () => $"linear score function does not have minimum score of 0 for n={n}: {format(scores)}");

        var diff = difference(scores);
        for (var i = 0; i < diff.Count() - 1; i++)
          Assert.That(diff[i] - diff[i + 1], Is.EqualTo(0).Within(0.000000000000001),
            () => $"linear score function is not monotonically decreasing for n={n}: out by {diff[i] - diff[i + 1]} {format(diff)}");
      });

      Assert.Pass();
    }

    [Test]
    public void Assert_That_Exponential_Scoring_Works()
    {
      random(1, 2, 10)
        .ForEach(b =>
        {
          Enumerable.Range(3, 11)
          .ToList()
          .ForEach(n =>
          {
            var scoringMethod = Scoring.Create(b);
            var scores = scoringMethod(n);

            Assert.That(scores.Sum(), Is.EqualTo(1).Within(0.000000000000001),
              () => $"exponential scoring algorithm does not sum to 1 for base={b} n={n}: {format(scores)}");

            Assert.That(scores.Min(), Is.EqualTo(0),
              () => $"exponential score function does not have minimum score of 0 for base={b} n={n}: {format(scores)}");

            var diff = difference(scores);
            for (var i = 0; i < diff.Count() - 1; i++)
              Assert.That(diff[i] - diff[i + 1], Is.GreaterThan(0).Within(0.000000000000001),
                () => $"exponential score function is not monotonically decreasing for n={n}: out by {diff[i] - diff[i + 1]} {format(diff)}");

            var diff_diffs = difference(scores);
            for (var i = 0; i < diff_diffs.Count(); i++)
              Assert.That(diff[i], Is.GreaterThan(0).Within(0.000000000000001),
                () => $"exponential score function is not monotonically decreasing for n={n}: out by {diff[i]} {format(diff)}");
          });
        });
    }
    
    //3 - 2.973957279878059
    [Test]
    public void Assert_That_Exponential_Scoring_Works_Matching_A_Python_Run()
    {
      var scoringMethod = Scoring.Create(2.973957279878059);
      var scores = scoringMethod(3);

      Assert.That(scores.Sum(), Is.EqualTo(1).Within(0.000000000000001),
        () => $"exponential scoring algorithm does not sum to 1 for base=2.973957279878059 n=3: {format(scores)}");

      Assert.That(scores.Min(), Is.EqualTo(0),
        () => $"exponential score function does not have minimum score of 0 for base=2.973957279878059 n=3: {format(scores)}");
    }

    
  }
}