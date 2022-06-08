namespace ThirdParty.Tests
{
    using System;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class CancellationsTests
    {
        [Fact]
        public void Solidify_Should_AddANewZeroCostCanxToStart_When_CancellationsStartAfterToday()
        {
            // Arrange
            var cancellations = new Cancellations
            {
                new Cancellation()
                {
                    Amount = 100,
                    StartDate = DateTime.Now.AddDays(7).Date,
                    EndDate = DateTime.Now.AddDays(14).Date
                }
            };

            // Act
            cancellations.Solidify(SolidifyType.Sum);

            // Assert
            Assert.Equal(2, cancellations.Count);
            Assert.Equal(DateTime.Now.Date, cancellations[0].StartDate);
            Assert.Equal(0, cancellations[0].Amount);
            Assert.Equal(DateTime.Now.AddDays(7).Date, cancellations[0].EndDate);

            Assert.Equal(DateTime.Now.AddDays(7).Date, cancellations[1].StartDate);
            Assert.Equal(100, cancellations[1].Amount);
            Assert.Equal(DateTime.Now.AddDays(14).Date, cancellations[1].EndDate);
        }

        [Fact]
        public void Solidify_Should_NotAddANewZeroCostCanxToStart_When_CancellationsStartToday()
        {
            // Arrange
            var cancellations = new Cancellations();
            cancellations.Add(new Cancellation()
            {
                Amount = 100,
                StartDate = DateTime.Now.AddDays(0).Date,
                EndDate = DateTime.Now.AddDays(7).Date
            });

            // Act
            cancellations.Solidify(SolidifyType.Sum);

            // Assert
            Assert.Single(cancellations);
            Assert.Equal(DateTime.Now.Date, cancellations[0].StartDate);
            Assert.Equal(100, cancellations[0].Amount);
            Assert.Equal(DateTime.Now.AddDays(7).Date, cancellations[0].EndDate);
        }

        [Fact]
        public void Solidify_Should_CombineAndSumCancelaltionPolicies_When_ThereIsOverlapAndTypeIsSum()
        {
            // Arrange
            var cancellations = new Cancellations();
            cancellations.Add(new Cancellation()
            {
                Amount = 100,
                StartDate = DateTime.Now.AddDays(0).Date,
                EndDate = DateTime.Now.AddDays(7).Date
            });
            cancellations.Add(new Cancellation()
            {
                Amount = 50,
                StartDate = DateTime.Now.AddDays(2).Date,
                EndDate = DateTime.Now.AddDays(7).Date
            });

            // Act
            cancellations.Solidify(SolidifyType.Sum);

            // Assert
            Assert.Equal(2, cancellations.Count);
            Assert.Equal(DateTime.Now.Date, cancellations[0].StartDate);
            Assert.Equal(100, cancellations[0].Amount);
            Assert.Equal(DateTime.Now.AddDays(1).Date, cancellations[0].EndDate);

            Assert.Equal(DateTime.Now.AddDays(2).Date, cancellations[1].StartDate);
            Assert.Equal(150, cancellations[1].Amount);
            Assert.Equal(DateTime.Now.AddDays(7).Date, cancellations[1].EndDate);
        }

        [Fact]
        public void Solidify_Should_CombineAndTakeHighestCancelaltionPolicies_When_ThereIsOverlapAndTypeIsMax()
        {
            // Arrange
            var cancellations = new Cancellations();
            cancellations.Add(new Cancellation()
            {
                Amount = 50,
                StartDate = DateTime.Now.AddDays(0).Date,
                EndDate = DateTime.Now.AddDays(7).Date
            });
            cancellations.Add(new Cancellation()
            {
                Amount = 100,
                StartDate = DateTime.Now.AddDays(2).Date,
                EndDate = DateTime.Now.AddDays(7).Date
            });

            // Act
            cancellations.Solidify(SolidifyType.Max);

            // Assert
            Assert.Equal(2, cancellations.Count);
            Assert.Equal(DateTime.Now.Date, cancellations[0].StartDate);
            Assert.Equal(50, cancellations[0].Amount);
            Assert.Equal(DateTime.Now.AddDays(1).Date, cancellations[0].EndDate);

            Assert.Equal(DateTime.Now.AddDays(2).Date, cancellations[1].StartDate);
            Assert.Equal(100, cancellations[1].Amount);
            Assert.Equal(DateTime.Now.AddDays(7).Date, cancellations[1].EndDate);
        }
    }
}