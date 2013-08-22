﻿// <copyright file="LogNormal.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2013 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Properties;
using MathNet.Numerics.Statistics;

namespace MathNet.Numerics.Distributions
{
    /// <summary>
    /// Continuous Univariate Log-Normal distribution.
    /// For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Log-normal_distribution">Wikipedia - Log-Normal distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public class LogNormal : IContinuousDistribution
    {
        System.Random _random;

        double _mu;
        double _sigma;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogNormal"/> class. 
        /// The distribution will be initialized with the default <seealso cref="System.Random"/>
        /// random number generator.
        /// </summary>
        /// <param name="mu">The log-scale (μ) of the logarithm of the distribution.</param>
        /// <param name="sigma">The shape (σ) of the logarithm of the distribution.</param>
        public LogNormal(double mu, double sigma)
        {
            _random = new System.Random();
            SetParameters(mu, sigma);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogNormal"/> class. 
        /// The distribution will be initialized with the default <seealso cref="System.Random"/>
        /// random number generator.
        /// </summary>
        /// <param name="mu">The log-scale (μ) of the distribution.</param>
        /// <param name="sigma">The shape (σ) of the distribution.</param>
        /// <param name="randomSource">The random number generator which is used to draw random samples.</param>
        public LogNormal(double mu, double sigma, System.Random randomSource)
        {
            _random = randomSource ?? new System.Random();
            SetParameters(mu, sigma);
        }

        /// <summary>
        /// Constructs a log-normal distribution with the desired mean and variance. The distribution will
        /// be initialized with the default <seealso cref="System.Random"/> random number generator.
        /// </summary>
        /// <param name="mean">The mean of the log-normal distribution.</param>
        /// <param name="var">The variance of the log-normal distribution.</param>
        /// <returns>a log-normal distribution.</returns>
        public static LogNormal WithMeanVariance(double mean, double var)
        {
            var sigma2 = Math.Log(var/(mean*mean) + 1.0);
            return new LogNormal(Math.Log(mean) - sigma2/2.0, Math.Sqrt(sigma2));
        }

        /// <summary>
        /// Estimates the log-normal distribution parameters from sample data with maximum-likelihood.
        /// </summary>
        public static LogNormal Estimate(IEnumerable<double> samples)
        {
            var muSigma2 = samples.Select(s => Math.Log(s)).MeanVariance();
            return new LogNormal(muSigma2.Item1, Math.Sqrt(muSigma2.Item2));
        }

        /// <summary>
        /// A string representation of the distribution.
        /// </summary>
        /// <returns>a string representation of the distribution.</returns>
        public override string ToString()
        {
            return "LogNormal(μ = " + _mu + ", σ = " + _sigma + ")";
        }

        /// <summary>
        /// Checks whether the parameters of the distribution are valid. 
        /// </summary>
        /// <param name="mu">The log-scale (μ) of the distribution.</param>
        /// <param name="sigma">The shape (σ) of the distribution.</param>
        /// <returns><c>true</c> when the parameters are valid, <c>false</c> otherwise.</returns>
        static bool IsValidParameterSet(double mu, double sigma)
        {
            return sigma >= 0.0 && !Double.IsNaN(mu);
        }

        /// <summary>
        /// Sets the parameters of the distribution after checking their validity.
        /// </summary>
        /// <param name="mu">The log-scale (μ) of the distribution.</param>
        /// <param name="sigma">The shape (σ) of the distribution.</param>
        /// <exception cref="ArgumentOutOfRangeException">When the parameters don't pass the <see cref="IsValidParameterSet"/> function.</exception>
        void SetParameters(double mu, double sigma)
        {
            if (Control.CheckDistributionParameters && !IsValidParameterSet(mu, sigma))
            {
                throw new ArgumentOutOfRangeException(Resources.InvalidDistributionParameters);
            }

            _mu = mu;
            _sigma = sigma;
        }

        /// <summary>
        /// Gets or sets the log-scale (μ) (mean of the logarithm) of the distribution.
        /// </summary>
        public double Mu
        {
            get { return _mu; }
            set { SetParameters(value, _sigma); }
        }

        /// <summary>
        /// Gets or sets the shape (σ) (standard deviation of the logarithm) of the distribution.
        /// </summary>
        public double Sigma
        {
            get { return _sigma; }
            set { SetParameters(_mu, value); }
        }

        /// <summary>
        /// Gets or sets the random number generator which is used to draw random samples.
        /// </summary>
        public System.Random RandomSource
        {
            get { return _random; }
            set { _random = value ?? new System.Random(); }
        }

        /// <summary>
        /// Gets the mu of the log-normal distribution.
        /// </summary>
        public double Mean
        {
            get { return Math.Exp(_mu + (_sigma*_sigma/2.0)); }
        }

        /// <summary>
        /// Gets the variance of the log-normal distribution.
        /// </summary>
        public double Variance
        {
            get
            {
                var sigma2 = _sigma*_sigma;
                return (Math.Exp(sigma2) - 1.0)*Math.Exp(_mu + _mu + sigma2);
            }
        }

        /// <summary>
        /// Gets the standard deviation of the log-normal distribution.
        /// </summary>
        public double StdDev
        {
            get
            {
                var sigma2 = _sigma*_sigma;
                return Math.Sqrt((Math.Exp(sigma2) - 1.0)*Math.Exp(_mu + _mu + sigma2));
            }
        }

        /// <summary>
        /// Gets the entropy of the log-normal distribution.
        /// </summary>
        public double Entropy
        {
            get { return 0.5 + Math.Log(_sigma) + _mu + Constants.LogSqrt2Pi; }
        }

        /// <summary>
        /// Gets the skewness of the log-normal distribution.
        /// </summary>
        public double Skewness
        {
            get
            {
                var expsigma2 = Math.Exp(_sigma*_sigma);
                return (expsigma2 + 2.0)*Math.Sqrt(expsigma2 - 1);
            }
        }

        /// <summary>
        /// Gets the mode of the log-normal distribution.
        /// </summary>
        public double Mode
        {
            get { return Math.Exp(_mu - (_sigma*_sigma)); }
        }

        /// <summary>
        /// Gets the median of the log-normal distribution.
        /// </summary>
        public double Median
        {
            get { return Math.Exp(_mu); }
        }

        /// <summary>
        /// Gets the minimum of the log-normal distribution.
        /// </summary>
        public double Minimum
        {
            get { return 0.0; }
        }

        /// <summary>
        /// Gets the maximum of the log-normal distribution.
        /// </summary>
        public double Maximum
        {
            get { return Double.PositiveInfinity; }
        }

        /// <summary>
        /// Computes the probability density of the distribution (PDF) at x, i.e. dP(X &lt;= x)/dx.
        /// </summary>
        /// <param name="x">The location at which to compute the density.</param>
        /// <returns>the density at <paramref name="x"/>.</returns>
        public double Density(double x)
        {
            if (x < 0.0)
            {
                return 0.0;
            }

            var a = (Math.Log(x) - _mu)/_sigma;
            return Math.Exp(-0.5*a*a)/(x*_sigma*Constants.Sqrt2Pi);
        }

        /// <summary>
        /// Computes the log probability density of the distribution (lnPDF) at x, i.e. ln(dP(X &lt;= x)/dx).
        /// </summary>
        /// <param name="x">The location at which to compute the log density.</param>
        /// <returns>the log density at <paramref name="x"/>.</returns>
        public double DensityLn(double x)
        {
            if (x < 0.0)
            {
                return Double.NegativeInfinity;
            }

            var a = (Math.Log(x) - _mu)/_sigma;
            return (-0.5*a*a) - Math.Log(x*_sigma) - Constants.LogSqrt2Pi;
        }

        /// <summary>
        /// Computes the cumulative distribution (CDF) of the distribution at x, i.e. P(X &lt;= x).
        /// </summary>
        /// <param name="x">The location at which to compute the cumulative distribution function.</param>
        /// <returns>the cumulative distribution at location <paramref name="x"/>.</returns>
        public double CumulativeDistribution(double x)
        {
            if (x < 0.0)
            {
                return 0.0;
            }

            return 0.5*(1.0 + SpecialFunctions.Erf((Math.Log(x) - _mu)/(_sigma*Constants.Sqrt2)));
        }

        /// <summary>
        /// Generates a sample from the log-normal distribution using the <i>Box-Muller</i> algorithm.
        /// </summary>
        /// <returns>a sample from the distribution.</returns>
        public double Sample()
        {
            return Math.Exp(Normal.SampleUnchecked(_random, _mu, _sigma));
        }

        /// <summary>
        /// Generates a sequence of samples from the log-normal distribution using the <i>Box-Muller</i> algorithm.
        /// </summary>
        /// <returns>a sequence of samples from the distribution.</returns>
        public IEnumerable<double> Samples()
        {
            return Normal.SamplesUnchecked(_random, _mu, _sigma).Select(Math.Exp);
        }

        /// <summary>
        /// Generates a sample from the log-normal distribution using the <i>Box-Muller</i> algorithm.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="mu">The log-scale (μ) of the distribution.</param>
        /// <param name="sigma">The shape (σ) of the distribution.</param>
        /// <returns>a sample from the distribution.</returns>
        public static double Sample(System.Random rnd, double mu, double sigma)
        {
            return Math.Exp(Normal.Sample(rnd, mu, sigma));
        }

        /// <summary>
        /// Generates a sequence of samples from the log-normal distribution using the <i>Box-Muller</i> algorithm.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="mu">The log-scale (μ) of the distribution.</param>
        /// <param name="sigma">The shape (σ) of the distribution.</param>
        /// <returns>a sequence of samples from the distribution.</returns>
        public static IEnumerable<double> Samples(System.Random rnd, double mu, double sigma)
        {
            return Normal.Samples(rnd, mu, sigma).Select(Math.Exp);
        }
    }
}
