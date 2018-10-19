﻿/*
 * Copyright (C) 2009 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PhoneNumbers.Test
{
    /**
    * Unit tests for PhoneNumberUtil.java
    *
    * Note that these tests use the test metadata, not the normal metadata file, so should not be used
    * for regression test purposes - these tests are illustrative only and test functionality.
    *
    * @author Shaopeng Jia
    * @author Lara Rennie
    */
    [Collection("TestMetadataTestCase")]
    public class TestPhoneNumberUtil : IClassFixture<TestMetadataTestCase>
    {
        private readonly PhoneNumberUtil phoneUtil;

        public TestPhoneNumberUtil(TestMetadataTestCase metadata)
        {
            phoneUtil = metadata.PhoneUtil;
        }

        // Set up some test numbers to re-use.
        private static readonly PhoneNumber AlphaNumericNumber =
            new PhoneNumber {CountryCode =1, NationalNumber = 80074935247L };
        private static readonly PhoneNumber ARMobile =
            new PhoneNumber {CountryCode = 54, NationalNumber = 91187654321L };
        private static readonly PhoneNumber ARNumber =
            new PhoneNumber {CountryCode = 54, NationalNumber = 1187654321 };
        private static readonly PhoneNumber AUNumber =
            new PhoneNumber {CountryCode = 61, NationalNumber = 236618300L };
        private static readonly PhoneNumber BSMobile =
            new PhoneNumber {CountryCode =1, NationalNumber = 2423570000L };
        private static readonly PhoneNumber BSNumber =
            new PhoneNumber {CountryCode =1, NationalNumber = 2423651234L };
        // Note that this is the same as the example number for DE in the metadata.
        private static readonly PhoneNumber DENumber =
            new PhoneNumber {CountryCode = 49, NationalNumber = 30123456L };
        private static readonly PhoneNumber DEMobile =
            new PhoneNumber {CountryCode = 49, NationalNumber = 15123456789L };
        private static readonly PhoneNumber DEShortNumber =
            new PhoneNumber {CountryCode = 49, NationalNumber = 1234L };
        private static readonly PhoneNumber GBMobile =
            new PhoneNumber {CountryCode = 44, NationalNumber = 7912345678L };
        private static readonly PhoneNumber GBNumber =
            new PhoneNumber {CountryCode = 44, NationalNumber = 2070313000L };
        private static readonly PhoneNumber ITMobile =
            new PhoneNumber {CountryCode = 39, NationalNumber = 345678901L };

        private static readonly PhoneNumber ITNumber =
            new PhoneNumber {CountryCode = 39, NationalNumber = 236618300L, ItalianLeadingZero = true};
        private static readonly PhoneNumber JPStarNumber =
            new PhoneNumber {CountryCode = 81, NationalNumber = 2345 };
        // Numbers to test the formatting rules from Mexico.
        private static readonly PhoneNumber MXMobile1 =
            new PhoneNumber {CountryCode = 52, NationalNumber = 12345678900L };
        private static readonly PhoneNumber MXMobile2 =
            new PhoneNumber {CountryCode = 52, NationalNumber = 15512345678L };
        private static readonly PhoneNumber MXNumber1 =
            new PhoneNumber {CountryCode = 52, NationalNumber = 3312345678L };
        private static readonly PhoneNumber MXNumber2 =
            new PhoneNumber {CountryCode = 52, NationalNumber = 8211234567L };
        private static readonly PhoneNumber NZNumber =
            new PhoneNumber {CountryCode = 64, NationalNumber = 33316005L };
        private static readonly PhoneNumber SGNumber =
            new PhoneNumber {CountryCode = 65, NationalNumber = 65218000L };
        // A too-long and hence invalid US number.
        private static readonly PhoneNumber USLongNumber =
            new PhoneNumber {CountryCode =1, NationalNumber = 65025300001L };
        private static readonly PhoneNumber USNumber =
            new PhoneNumber {CountryCode =1, NationalNumber = 6502530000L };
        private static readonly PhoneNumber USPremium =
            new PhoneNumber {CountryCode =1, NationalNumber = 9002530000L };
        // Too short, but still possible US numbers.
        private static readonly PhoneNumber USLocalNumber =
            new PhoneNumber {CountryCode =1, NationalNumber = 2530000L };
        private static readonly PhoneNumber USShortByOneNumber =
            new PhoneNumber {CountryCode =1, NationalNumber = 650253000L };
        private static readonly PhoneNumber USTollfree =
            new PhoneNumber {CountryCode =1, NationalNumber = 8002530000L };
        private static readonly PhoneNumber USSpoof =
            new PhoneNumber {CountryCode =1, NationalNumber = 0L };

        private static readonly PhoneNumber USSpoofWithRawInput =
            new PhoneNumber {CountryCode = 1, NationalNumber = 0L, RawInput = "000-000-0000"};
        private static readonly PhoneNumber InternationalTollFree =
            new PhoneNumber {CountryCode = 800, NationalNumber = 12345678L };
        // We set this to be the same length as numbers for the other non-geographical country prefix that
        // we have in our test metadata. However, this is not considered valid because they differ in
        // their country calling code.
        private static readonly PhoneNumber InternationalTollFreeTooLong =
            new PhoneNumber {CountryCode = 800, NationalNumber = 1234567890L };
        private static readonly PhoneNumber UniversalPremiumRate =
            new PhoneNumber {CountryCode = 979, NationalNumber = 123456789L };


        private static PhoneNumber Update(PhoneNumber p)
        {
            return new PhoneNumber().MergeFrom(p);
        }

        private static NumberFormat Update(NumberFormat p)
        {
            return new NumberFormat().MergeFrom(p);
        }

        private static PhoneMetadata Update(PhoneMetadata p)
        {
            return new PhoneMetadata().MergeFrom(p);
        }

        private static void AreEqual(PhoneNumber p1, PhoneNumber p2)
        {
            Assert.Equal(p1, p2);
        }

        [Fact]
        public void TestGetSupportedRegions()
        {
            Assert.True(phoneUtil.GetSupportedRegions().Count > 0);
        }

        [Fact]
        public void TestGetInstanceLoadUSMetadata()
        {
            var metadata = phoneUtil.GetMetadataForRegion(RegionCode.US);
            Assert.Equal("US", metadata.Id);
            Assert.Equal(1, metadata.CountryCode);
            Assert.Equal("011", metadata.InternationalPrefix);
            Assert.True(metadata.NationalPrefix != null);
            Assert.Equal(2, metadata.NumberFormats.Count);
            Assert.Equal("(\\d{3})(\\d{3})(\\d{4})",
                metadata.NumberFormats[1].Pattern);
            Assert.Equal("$1 $2 $3", metadata.NumberFormats[1].Format);
            Assert.Equal("[13-689]\\d{9}|2[0-35-9]\\d{8}",
                metadata.GeneralDesc.NationalNumberPattern);
            Assert.Equal("[13-689]\\d{9}|2[0-35-9]\\d{8}",
                metadata.FixedLine.NationalNumberPattern);
            Assert.Equal(1, metadata.GeneralDesc.PossibleLengths.Count);
            Assert.Equal(10, metadata.GeneralDesc.PossibleLengths[0]);
            // Possible lengths are the same as the general description, so aren't stored separately in the
            // toll free element as well.
            Assert.Equal(0, metadata.TollFree.PossibleLengths.Count);
            Assert.Equal("900\\d{7}", metadata.PremiumRate.NationalNumberPattern);
            // No shared-cost data is available, so its national number data should not be set.
            Assert.False(metadata.SharedCost.NationalNumberPattern != null);
        }

        [Fact]
        public void TestGetInstanceLoadDEMetadata()
        {
            var metadata = phoneUtil.GetMetadataForRegion(RegionCode.DE);
            Assert.Equal("DE", metadata.Id);
            Assert.Equal(49, metadata.CountryCode);
            Assert.Equal("00", metadata.InternationalPrefix);
            Assert.Equal("0", metadata.NationalPrefix);
            Assert.Equal(6, metadata.NumberFormats.Count);
            Assert.Equal(1, metadata.NumberFormats[5].LeadingDigitsPatterns.Count);
            Assert.Equal("900", metadata.NumberFormats[5].LeadingDigitsPatterns[0]);
            Assert.Equal("(\\d{3})(\\d{3,4})(\\d{4})",
                metadata.NumberFormats[5].Pattern);
            Assert.Equal("$1 $2 $3", metadata.NumberFormats[5].Format);
            Assert.Equal(2, metadata.GeneralDesc.PossibleLengthsLocalOnly.Count);
            Assert.Equal(8, metadata.GeneralDesc.PossibleLengths.Count);
            // Nothing is present for fixed-line, since it is the same as the general desc, so for
            // efficiency reasons we don't store an extra value.
            Assert.Equal(0, metadata.FixedLine.PossibleLengths.Count);
            Assert.Equal(2, metadata.Mobile.PossibleLengths.Count);
            Assert.Equal("(?:[24-6]\\d{2}|3[03-9]\\d|[789](?:0[2-9]|[1-9]\\d))\\d{1,8}",
                metadata.FixedLine.NationalNumberPattern);
            Assert.Equal("30123456", metadata.FixedLine.ExampleNumber);
            Assert.Equal(10, metadata.TollFree.PossibleLengths[0]);
            Assert.Equal("900([135]\\d{6}|9\\d{7})", metadata.PremiumRate.NationalNumberPattern);
        }

        [Fact]
        public void TestGetInstanceLoadARMetadata()
        {
            var metadata = phoneUtil.GetMetadataForRegion(RegionCode.AR);
            Assert.Equal("AR", metadata.Id);
            Assert.Equal(54, metadata.CountryCode);
            Assert.Equal("00", metadata.InternationalPrefix);
            Assert.Equal("0", metadata.NationalPrefix);
            Assert.Equal("0(?:(11|343|3715)15)?", metadata.NationalPrefixForParsing);
            Assert.Equal("9$1", metadata.NationalPrefixTransformRule);
            Assert.Equal("$2 15 $3-$4", metadata.NumberFormats[2].Format);
            Assert.Equal("(9)(\\d{4})(\\d{2})(\\d{4})",
                     metadata.NumberFormats[3].Pattern);
            Assert.Equal("(9)(\\d{4})(\\d{2})(\\d{4})",
                     metadata.IntlNumberFormats[3].Pattern);
            Assert.Equal("$1 $2 $3 $4", metadata.IntlNumberFormats[3].Format);
        }

        [Fact]
        public void TestGetInstanceLoadInternationalTollFreeMetadata()
        {
            var metadata = phoneUtil.GetMetadataForNonGeographicalRegion(800);
            Assert.Equal("001", metadata.Id);
            Assert.Equal(800, metadata.CountryCode);
            Assert.Equal("$1 $2", metadata.NumberFormats[0].Format);
            Assert.Equal("(\\d{4}\\d{4})", metadata.NumberFormats[0].Pattern);
            Assert.Equal("12345678", metadata.TollFree.ExampleNumber);
        }

        [Fact]
        public void TestGetLengthOfGeographicalAreaCode()
        {
            // Google MTV, which has area code "650".
            Assert.Equal(3, phoneUtil.GetLengthOfGeographicalAreaCode(USNumber));

            // A North America toll-free number, which has no area code.
            Assert.Equal(0, phoneUtil.GetLengthOfGeographicalAreaCode(USTollfree));

            // Google London, which has area code "20".
            Assert.Equal(2, phoneUtil.GetLengthOfGeographicalAreaCode(GBNumber));

            // A UK mobile phone, which has no area code.
            Assert.Equal(0, phoneUtil.GetLengthOfGeographicalAreaCode(GBMobile));

            // Google Buenos Aires, which has area code "11".
            Assert.Equal(2, phoneUtil.GetLengthOfGeographicalAreaCode(ARNumber));

            // Google Sydney, which has area code "2".
            Assert.Equal(1, phoneUtil.GetLengthOfGeographicalAreaCode(AUNumber));

            // Italian numbers - there is no national prefix, but it still has an area code.
            Assert.Equal(2, phoneUtil.GetLengthOfGeographicalAreaCode(ITNumber));

            // Google Singapore. Singapore has no area code and no national prefix.
            Assert.Equal(0, phoneUtil.GetLengthOfGeographicalAreaCode(SGNumber));

            // An invalid US number (1 digit shorter), which has no area code.
            Assert.Equal(0, phoneUtil.GetLengthOfGeographicalAreaCode(USShortByOneNumber));

            // An international toll free number, which has no area code.
            Assert.Equal(0, phoneUtil.GetLengthOfGeographicalAreaCode(InternationalTollFree));
        }

        [Fact]
        public void TestGetLengthOfNationalDestinationCode()
        {
            // Google MTV, which has national destination code (NDC) "650".
            Assert.Equal(3, phoneUtil.GetLengthOfNationalDestinationCode(USNumber));

            // A North America toll-free number, which has NDC "800".
            Assert.Equal(3, phoneUtil.GetLengthOfNationalDestinationCode(USTollfree));

            // Google London, which has NDC "20".
            Assert.Equal(2, phoneUtil.GetLengthOfNationalDestinationCode(GBNumber));

            // A UK mobile phone, which has NDC "7912".
            Assert.Equal(4, phoneUtil.GetLengthOfNationalDestinationCode(GBMobile));

            // Google Buenos Aires, which has NDC "11".
            Assert.Equal(2, phoneUtil.GetLengthOfNationalDestinationCode(ARNumber));

            // An Argentinian mobile which has NDC "911".
            Assert.Equal(3, phoneUtil.GetLengthOfNationalDestinationCode(ARMobile));

            // Google Sydney, which has NDC "2".
            Assert.Equal(1, phoneUtil.GetLengthOfNationalDestinationCode(AUNumber));

            // Google Singapore, which has NDC "6521".
            Assert.Equal(4, phoneUtil.GetLengthOfNationalDestinationCode(SGNumber));

            // An invalid US number (1 digit shorter), which has no NDC.
            Assert.Equal(0, phoneUtil.GetLengthOfNationalDestinationCode(USShortByOneNumber));

            // A number containing an invalid country calling code, which shouldn't have any NDC.
            var number = new PhoneNumber {CountryCode = 123, NationalNumber = 6502530000L };
            Assert.Equal(0, phoneUtil.GetLengthOfNationalDestinationCode(number));
        }

        [Fact]
        public void TestGetNationalSignificantNumber()
        {
            Assert.Equal("6502530000", phoneUtil.GetNationalSignificantNumber(USNumber));

            // An Italian mobile number.
            Assert.Equal("345678901", phoneUtil.GetNationalSignificantNumber(ITMobile));

            // An Italian fixed line number.
            Assert.Equal("0236618300", phoneUtil.GetNationalSignificantNumber(ITNumber));

            Assert.Equal("12345678", phoneUtil.GetNationalSignificantNumber(InternationalTollFree));
        }

        [Fact]
        public void TestGetExampleNumber()
        {
            Assert.Equal(DENumber, phoneUtil.GetExampleNumber(RegionCode.DE));

            Assert.Equal(DENumber, phoneUtil.GetExampleNumberForType(RegionCode.DE,
                PhoneNumberType.FIXED_LINE));
            Assert.Equal(DEMobile, phoneUtil.GetExampleNumberForType(RegionCode.DE,
                PhoneNumberType.MOBILE));
            // For the US, the example number is placed under general description, and hence should be used
            // for both fixed line and mobile, so neither of these should return null.
            Assert.NotNull(phoneUtil.GetExampleNumberForType(RegionCode.US,
                PhoneNumberType.FIXED_LINE));
            Assert.NotNull(phoneUtil.GetExampleNumberForType(RegionCode.US,
                PhoneNumberType.MOBILE));
            // CS is an invalid region, so we have no data for it.
            Assert.Null(phoneUtil.GetExampleNumberForType(RegionCode.CS,
                PhoneNumberType.MOBILE));
            // RegionCode 001 is reserved for supporting non-geographical country calling code. We don't
            // support getting an example number for it with this method.
            Assert.Equal(null, phoneUtil.GetExampleNumber(RegionCode.UN001));
        }

        [Fact]
        public void TestConvertAlphaCharactersInNumber()
        {
            var input = "1800-ABC-DEF";
            // Alpha chars are converted to digits; everything else is left untouched.
            var expectedOutput = "1800-222-333";
            Assert.Equal(expectedOutput, PhoneNumberUtil.ConvertAlphaCharactersInNumber(input));
        }


        [Fact]
        public void TestNormaliseRemovePunctuation()
        {
            var inputNumber = "034-56&+#2\u00AD34";
            var expectedOutput = "03456234";
            Assert.Equal(expectedOutput,
                PhoneNumberUtil.Normalize(inputNumber));
        }

        [Fact]
        public void TestNormaliseReplaceAlphaCharacters()
        {
            var inputNumber = "034-I-am-HUNGRY";
            var expectedOutput = "034426486479";
            Assert.Equal(expectedOutput,
                PhoneNumberUtil.Normalize(inputNumber));
        }

        [Fact]
        public void TestNormaliseOtherDigits()
        {
            var inputNumber = "\uFF125\u0665";
            var expectedOutput = "255";
            Assert.Equal(expectedOutput,
                PhoneNumberUtil.Normalize(inputNumber));
            // Eastern-Arabic digits.
            inputNumber = "\u06F52\u06F0";
            expectedOutput = "520";
            Assert.Equal(expectedOutput,
                PhoneNumberUtil.Normalize(inputNumber));
        }

        [Fact]
        public void TestNormaliseStripAlphaCharacters()
        {
            var inputNumber = "034-56&+a#234";
            var expectedOutput = "03456234";
            Assert.Equal(expectedOutput,
                PhoneNumberUtil.NormalizeDigitsOnly(inputNumber));
        }

        [Fact]
        public void TestFormatUSNumber()
        {
            Assert.Equal("650 253 0000", phoneUtil.Format(USNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+1 650 253 0000", phoneUtil.Format(USNumber, PhoneNumberFormat.INTERNATIONAL));

            Assert.Equal("800 253 0000", phoneUtil.Format(USTollfree, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+1 800 253 0000", phoneUtil.Format(USTollfree, PhoneNumberFormat.INTERNATIONAL));

            Assert.Equal("900 253 0000", phoneUtil.Format(USPremium, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+1 900 253 0000", phoneUtil.Format(USPremium, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("tel:+1-900-253-0000", phoneUtil.Format(USPremium, PhoneNumberFormat.RFC3966));

            // Numbers with all zeros in the national number part will be formatted by using the raw_input
            // if that is available no matter which format is specified.
            Assert.Equal("000-000-0000",
                phoneUtil.Format(USSpoofWithRawInput, PhoneNumberFormat.NATIONAL));
            Assert.Equal("0", phoneUtil.Format(USSpoof, PhoneNumberFormat.NATIONAL));
        }

        [Fact]
        public void TestFormatBSNumber()
        {
            Assert.Equal("242 365 1234", phoneUtil.Format(BSNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+1 242 365 1234", phoneUtil.Format(BSNumber, PhoneNumberFormat.INTERNATIONAL));
        }

        [Fact]
        public void TestFormatGBNumber()
        {
            Assert.Equal("(020) 7031 3000", phoneUtil.Format(GBNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+44 20 7031 3000", phoneUtil.Format(GBNumber, PhoneNumberFormat.INTERNATIONAL));

            Assert.Equal("(07912) 345 678", phoneUtil.Format(GBMobile, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+44 7912 345 678", phoneUtil.Format(GBMobile, PhoneNumberFormat.INTERNATIONAL));
        }

        [Fact]
        public void TestFormatDENumber()
        {
            var deNumber = new PhoneNumber {CountryCode = 49, NationalNumber = 301234L };
            Assert.Equal("030/1234", phoneUtil.Format(deNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+49 30/1234", phoneUtil.Format(deNumber, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("tel:+49-30-1234", phoneUtil.Format(deNumber, PhoneNumberFormat.RFC3966));

            deNumber = new PhoneNumber {CountryCode = 49, NationalNumber = 291123L };
            Assert.Equal("0291 123", phoneUtil.Format(deNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+49 291 123", phoneUtil.Format(deNumber, PhoneNumberFormat.INTERNATIONAL));

            deNumber = new PhoneNumber {CountryCode = 49, NationalNumber = 29112345678L };
            Assert.Equal("0291 12345678", phoneUtil.Format(deNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+49 291 12345678", phoneUtil.Format(deNumber, PhoneNumberFormat.INTERNATIONAL));

            deNumber = new PhoneNumber {CountryCode = 49, NationalNumber = 912312345L };
            Assert.Equal("09123 12345", phoneUtil.Format(deNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+49 9123 12345", phoneUtil.Format(deNumber, PhoneNumberFormat.INTERNATIONAL));

            deNumber = new PhoneNumber {CountryCode = 49, NationalNumber = 80212345L };
            Assert.Equal("08021 2345", phoneUtil.Format(deNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+49 8021 2345", phoneUtil.Format(deNumber, PhoneNumberFormat.INTERNATIONAL));
            // Note this number is correctly formatted without national prefix. Most of the numbers that
            // are treated as invalid numbers by the library are short numbers, and they are usually not
            // dialed with national prefix.
            Assert.Equal("1234", phoneUtil.Format(DEShortNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+49 1234", phoneUtil.Format(DEShortNumber, PhoneNumberFormat.INTERNATIONAL));

            deNumber = new PhoneNumber {CountryCode = 49, NationalNumber = 41341234 };
            Assert.Equal("04134 1234", phoneUtil.Format(deNumber, PhoneNumberFormat.NATIONAL));
        }

        [Fact]
        public void TestFormatITNumber()
        {
            Assert.Equal("02 3661 8300", phoneUtil.Format(ITNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+39 02 3661 8300", phoneUtil.Format(ITNumber, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("+390236618300", phoneUtil.Format(ITNumber, PhoneNumberFormat.E164));

            Assert.Equal("345 678 901", phoneUtil.Format(ITMobile, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+39 345 678 901", phoneUtil.Format(ITMobile, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("+39345678901", phoneUtil.Format(ITMobile, PhoneNumberFormat.E164));
        }

        [Fact]
        public void TestFormatAUNumber()
        {
            Assert.Equal("02 3661 8300", phoneUtil.Format(AUNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+61 2 3661 8300", phoneUtil.Format(AUNumber, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("+61236618300", phoneUtil.Format(AUNumber, PhoneNumberFormat.E164));

            var auNumber = new PhoneNumber {CountryCode = 61, NationalNumber = 1800123456L };
            Assert.Equal("1800 123 456", phoneUtil.Format(auNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+61 1800 123 456", phoneUtil.Format(auNumber, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("+611800123456", phoneUtil.Format(auNumber, PhoneNumberFormat.E164));
        }

        [Fact]
        public void TestFormatARNumber()
        {
            Assert.Equal("011 8765-4321", phoneUtil.Format(ARNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+54 11 8765-4321", phoneUtil.Format(ARNumber, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("+541187654321", phoneUtil.Format(ARNumber, PhoneNumberFormat.E164));

            Assert.Equal("011 15 8765-4321", phoneUtil.Format(ARMobile, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+54 9 11 8765 4321", phoneUtil.Format(ARMobile,
                                                            PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("+5491187654321", phoneUtil.Format(ARMobile, PhoneNumberFormat.E164));
        }

        [Fact]
        public void TestFormatMXNumber()
        {
            Assert.Equal("045 234 567 8900", phoneUtil.Format(MXMobile1, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+52 1 234 567 8900", phoneUtil.Format(
                MXMobile1, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("+5212345678900", phoneUtil.Format(MXMobile1, PhoneNumberFormat.E164));

            Assert.Equal("045 55 1234 5678", phoneUtil.Format(MXMobile2, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+52 1 55 1234 5678", phoneUtil.Format(
                MXMobile2, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("+5215512345678", phoneUtil.Format(MXMobile2, PhoneNumberFormat.E164));

            Assert.Equal("01 33 1234 5678", phoneUtil.Format(MXNumber1, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+52 33 1234 5678", phoneUtil.Format(MXNumber1, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("+523312345678", phoneUtil.Format(MXNumber1, PhoneNumberFormat.E164));

            Assert.Equal("01 821 123 4567", phoneUtil.Format(MXNumber2, PhoneNumberFormat.NATIONAL));
            Assert.Equal("+52 821 123 4567", phoneUtil.Format(MXNumber2, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("+528211234567", phoneUtil.Format(MXNumber2, PhoneNumberFormat.E164));
        }

        [Fact]
        public void TestFormatOutOfCountryCallingNumber()
        {
            Assert.Equal("00 1 900 253 0000",
            phoneUtil.FormatOutOfCountryCallingNumber(USPremium, RegionCode.DE));

            Assert.Equal("1 650 253 0000",
            phoneUtil.FormatOutOfCountryCallingNumber(USNumber, RegionCode.BS));

            Assert.Equal("00 1 650 253 0000",
            phoneUtil.FormatOutOfCountryCallingNumber(USNumber, RegionCode.PL));

            Assert.Equal("011 44 7912 345 678",
            phoneUtil.FormatOutOfCountryCallingNumber(GBMobile, RegionCode.US));

            Assert.Equal("00 49 1234",
            phoneUtil.FormatOutOfCountryCallingNumber(DEShortNumber, RegionCode.GB));
            // Note this number is correctly formatted without national prefix. Most of the numbers that
            // are treated as invalid numbers by the library are short numbers, and they are usually not
            // dialed with national prefix.
            Assert.Equal("1234", phoneUtil.FormatOutOfCountryCallingNumber(DEShortNumber, RegionCode.DE));

            Assert.Equal("011 39 02 3661 8300",
            phoneUtil.FormatOutOfCountryCallingNumber(ITNumber, RegionCode.US));
            Assert.Equal("02 3661 8300",
            phoneUtil.FormatOutOfCountryCallingNumber(ITNumber, RegionCode.IT));
            Assert.Equal("+39 02 3661 8300",
            phoneUtil.FormatOutOfCountryCallingNumber(ITNumber, RegionCode.SG));

            Assert.Equal("6521 8000",
            phoneUtil.FormatOutOfCountryCallingNumber(SGNumber, RegionCode.SG));

            Assert.Equal("011 54 9 11 8765 4321",
            phoneUtil.FormatOutOfCountryCallingNumber(ARMobile, RegionCode.US));
            Assert.Equal("011 800 1234 5678",
                 phoneUtil.FormatOutOfCountryCallingNumber(InternationalTollFree, RegionCode.US));

            var arNumberWithExtn = new PhoneNumber().MergeFrom(ARMobile);
            arNumberWithExtn.Extension = "1234";
            Assert.Equal("011 54 9 11 8765 4321 ext. 1234",
            phoneUtil.FormatOutOfCountryCallingNumber(arNumberWithExtn, RegionCode.US));
            Assert.Equal("0011 54 9 11 8765 4321 ext. 1234",
            phoneUtil.FormatOutOfCountryCallingNumber(arNumberWithExtn, RegionCode.AU));
            Assert.Equal("011 15 8765-4321 ext. 1234",
            phoneUtil.FormatOutOfCountryCallingNumber(arNumberWithExtn, RegionCode.AR));
        }

        [Fact]
        public void TestFormatOutOfCountryWithInvalidRegion()
        {
            // AQ/Antarctica isn't a valid region code for phone number formatting,
            // so this falls back to intl formatting.
            Assert.Equal("+1 650 253 0000",
                phoneUtil.FormatOutOfCountryCallingNumber(USNumber, RegionCode.AQ));
            // For region code 001, the out-of-country format always turns into the international format.
            Assert.Equal("+1 650 253 0000",
                 phoneUtil.FormatOutOfCountryCallingNumber(USNumber, RegionCode.UN001));

        }

        [Fact]
        public void TestFormatOutOfCountryWithPreferredIntlPrefix()
        {
            // This should use 0011, since that is the preferred international prefix (both 0011 and 0012
            // are accepted as possible international prefixes in our test metadta.)
            Assert.Equal("0011 39 02 3661 8300",
                phoneUtil.FormatOutOfCountryCallingNumber(ITNumber, RegionCode.AU));
        }

        [Fact]
        public void TestFormatOutOfCountryKeepingAlphaChars()
        {
            var alphaNumericNumber =
                new PhoneNumber {CountryCode = 1, NationalNumber = 8007493524L, RawInput = "1800 six-flag"};
            Assert.Equal("0011 1 800 SIX-FLAG",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            alphaNumericNumber
                .RawInput = "1-800-SIX-flag";
            Assert.Equal("0011 1 800-SIX-FLAG",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            alphaNumericNumber.RawInput = "Call us from UK: 00 1 800 SIX-flag";
            ;
            Assert.Equal("0011 1 800 SIX-FLAG",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            alphaNumericNumber.RawInput = "800 SIX-flag";
            ;
            Assert.Equal("0011 1 800 SIX-FLAG",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            // Formatting from within the NANPA region.
            Assert.Equal("1 800 SIX-FLAG",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.US));

            Assert.Equal("1 800 SIX-FLAG",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.BS));

            // Testing that if the raw input doesn't exist, it is formatted using
            // FormatOutOfCountryCallingNumber.
            alphaNumericNumber.RawInput = null;
            Assert.Equal("00 1 800 749 3524",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.DE));

            // Testing AU alpha number formatted from Australia.
            alphaNumericNumber = new PhoneNumber
            {
                CountryCode = 61,
                NationalNumber = 827493524L,
                RawInput = "+61 82749-FLAG"
            };
            ;
            // This number should have the national prefix fixed.
            Assert.Equal("082749-FLAG",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            alphaNumericNumber.RawInput = "082749-FLAG";
            Assert.Equal("082749-FLAG",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            alphaNumericNumber.NationalNumber = 18007493524L;
            alphaNumericNumber.RawInput =
                "1-800-SIX-flag"; // This number should not have the national prefix prefixed, in accordance with the override for
            ;
            // this specific formatting rule.
            Assert.Equal("1-800-SIX-FLAG",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            // The metadata should not be permanently changed, since we copied it before modifying patterns.
            // Here we check this.
            alphaNumericNumber.NationalNumber = 1800749352L;
            Assert.Equal("1800 749 352", phoneUtil.FormatOutOfCountryCallingNumber(alphaNumericNumber, RegionCode.AU));

            // Testing a region with multiple international prefixes.
            Assert.Equal("+61 1-800-SIX-FLAG",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.SG));
            // Testing the case of calling from a non-supported region.
            Assert.Equal("+61 1-800-SIX-FLAG",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AQ));

            // Testing the case with an invalid country calling code.
            alphaNumericNumber =
                new PhoneNumber
                {
                    CountryCode = 0,
                    NationalNumber = 18007493524L,
                    RawInput = "1-800-SIX-flag"
                }; // Uses the raw input only.
            ;
            Assert.Equal("1-800-SIX-flag",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.DE));

            // Testing the case of an invalid alpha number.
            alphaNumericNumber = new PhoneNumber{CountryCode = 1, NationalNumber = 80749L, RawInput = "180-SIX"}; // No country-code stripping can be done.
        ;

        Assert.Equal("00 1 180-SIX",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.DE));

            // Testing the case of calling from a non-supported region.
            alphaNumericNumber = new PhoneNumber {CountryCode = 1, NationalNumber = 80749L, RawInput = "180-SIX"};            // No country-code stripping can be done since the number is invalid.

            Assert.Equal("+1 180-SIX",
                phoneUtil.FormatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AQ));
        }

        [Fact]
        public void TestFormatWithCarrierCode()
        {
            // We only support this for AR in our test metadata, and only for mobile numbers starting with
            // certain values.
            var arMobile = new PhoneNumber {CountryCode = 54, NationalNumber = 92234654321L };
            Assert.Equal("02234 65-4321", phoneUtil.Format(arMobile, PhoneNumberFormat.NATIONAL));
            // Here we force 14 as the carrier code.
            Assert.Equal("02234 14 65-4321",
                phoneUtil.FormatNationalNumberWithCarrierCode(arMobile, "14"));
            // Here we force the number to be shown with no carrier code.
            Assert.Equal("02234 65-4321",
                phoneUtil.FormatNationalNumberWithCarrierCode(arMobile, ""));
            // Here the international rule is used, so no carrier code should be present.
            Assert.Equal("+5492234654321", phoneUtil.Format(arMobile, PhoneNumberFormat.E164));
            // We don't support this for the US so there should be no change.
            Assert.Equal("650 253 0000", phoneUtil.FormatNationalNumberWithCarrierCode(USNumber, "15"));
        }

        [Fact]
        public void TestFormatWithPreferredCarrierCode()
        {
            // We only support this for AR in our test metadata.
            var arNumber = new PhoneNumber {CountryCode = 54, NationalNumber = 91234125678L};            // Test formatting with no preferred carrier code stored in the number itself.
            Assert.Equal("01234 15 12-5678",
                phoneUtil.FormatNationalNumberWithPreferredCarrierCode(arNumber, "15"));
            Assert.Equal("01234 12-5678",
            phoneUtil.FormatNationalNumberWithPreferredCarrierCode(arNumber, ""));
            // Test formatting with preferred carrier code present.
            arNumber.PreferredDomesticCarrierCode = "19";
            Assert.Equal("01234 19 12-5678",
                phoneUtil.FormatNationalNumberWithPreferredCarrierCode(arNumber, "15"));
            Assert.Equal("01234 19 12-5678",
                phoneUtil.FormatNationalNumberWithPreferredCarrierCode(arNumber, ""));
            // When the preferred_domestic_carrier_code is present (even when it is just a space), use it
            // instead of the default carrier code passed in.
            arNumber.PreferredDomesticCarrierCode = " ";
            Assert.Equal("01234   12-5678",
                phoneUtil.FormatNationalNumberWithPreferredCarrierCode(arNumber, "15"));
            // When the preferred_domestic_carrier_code is present but empty, treat it as unset and use
            // instead the default carrier code passed in.
            arNumber.PreferredDomesticCarrierCode = "";
            Assert.Equal("01234 15 12-5678",
                phoneUtil.FormatNationalNumberWithPreferredCarrierCode(arNumber, "15"));
            // We don't support this for the US so there should be no change.
            var usNumber = new PhoneNumber
            {
                CountryCode = 1,
                NationalNumber = 4241231234L,
                PreferredDomesticCarrierCode = "99"
            };
            Assert.Equal("424 123 1234", phoneUtil.Format(usNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal("424 123 1234",
            phoneUtil.FormatNationalNumberWithPreferredCarrierCode(usNumber, "15"));
        }

        [Fact]
        public void TestFormatNumberForMobileDialing()
        {
            // US toll free numbers are marked as noInternationalDialling in the test metadata for testing
            // purposes.
            Assert.Equal("800 253 0000",
                phoneUtil.FormatNumberForMobileDialing(USTollfree, RegionCode.US,
                    true /*  keep formatting */));
            Assert.Equal("", phoneUtil.FormatNumberForMobileDialing(USTollfree, RegionCode.CN, true));
            Assert.Equal("+1 650 253 0000",
                phoneUtil.FormatNumberForMobileDialing(USNumber, RegionCode.US, true));
            var usNumberWithExtn = new PhoneNumber().MergeFrom(USNumber);
            usNumberWithExtn.Extension = "1234";
            Assert.Equal("+1 650 253 0000",
                phoneUtil.FormatNumberForMobileDialing(usNumberWithExtn, RegionCode.US, true));

            Assert.Equal("8002530000",
                phoneUtil.FormatNumberForMobileDialing(USTollfree, RegionCode.US,
                    false /* remove formatting */));
            Assert.Equal("", phoneUtil.FormatNumberForMobileDialing(USTollfree, RegionCode.CN, false));
            Assert.Equal("+16502530000",
                phoneUtil.FormatNumberForMobileDialing(USNumber, RegionCode.US, false));
            Assert.Equal("+16502530000",
                phoneUtil.FormatNumberForMobileDialing(usNumberWithExtn, RegionCode.US, false));

            // An invalid US number, which is one digit too long.
            Assert.Equal("+165025300001",
                phoneUtil.FormatNumberForMobileDialing(USLongNumber, RegionCode.US, false));
            Assert.Equal("+1 65025300001",
                phoneUtil.FormatNumberForMobileDialing(USLongNumber, RegionCode.US, true));

            // Star numbers. In real life they appear in Israel, but we have them in JP in our test
            // metadata.
            Assert.Equal("*2345",
                phoneUtil.FormatNumberForMobileDialing(JPStarNumber, RegionCode.JP, false));
            Assert.Equal("*2345",
                phoneUtil.FormatNumberForMobileDialing(JPStarNumber, RegionCode.JP, true));

            Assert.Equal("+80012345678",
                phoneUtil.FormatNumberForMobileDialing(InternationalTollFree, RegionCode.JP, false));
            Assert.Equal("+800 1234 5678",
                phoneUtil.FormatNumberForMobileDialing(InternationalTollFree, RegionCode.JP, true));
        }

        [Fact]
        public void TestFormatByPattern()
        {
            var newNumFormat = new NumberFormat {Pattern = "(\\d{3}\\d{3})(\\d{4})", Format = "($1) $2-$3" };
            var newNumberFormats = new List<NumberFormat>();

            Assert.Equal("(650) 253-0000", phoneUtil.FormatByPattern(USNumber, PhoneNumberFormat.NATIONAL,
                newNumberFormats));
            Assert.Equal("+1 (650) 253-0000", phoneUtil.FormatByPattern(USNumber,
                PhoneNumberFormat.INTERNATIONAL,
                newNumberFormats));
            Assert.Equal("tel:+1-650-253-0000", phoneUtil.FormatByPattern(USNumber,
                PhoneNumberFormat.RFC3966, newNumberFormats));


            // $NP is set to '1' for the US. Here we check that for other NANPA countries the US rules are
            // followed.
            newNumberFormats[0].NationalPrefixFormattingRule = "$NP ($FG)";
;
            newNumberFormats[0].Format = "$1 $2-$3";
            Assert.Equal("1 (242) 365-1234",
                phoneUtil.FormatByPattern(BSNumber, PhoneNumberFormat.NATIONAL,
                newNumberFormats));
            Assert.Equal("+1 242 365-1234",
                phoneUtil.FormatByPattern(BSNumber, PhoneNumberFormat.INTERNATIONAL,
                newNumberFormats));

            newNumberFormats[0].Pattern = "(\\d{2}\\d{5})(\\d{3})";
;
            newNumberFormats[0].Format = "$1-$2 $3";
            Assert.Equal("02-36618 300",
                phoneUtil.FormatByPattern(ITNumber, PhoneNumberFormat.NATIONAL,
                newNumberFormats));
            Assert.Equal("+39 02-36618 300",
                phoneUtil.FormatByPattern(ITNumber, PhoneNumberFormat.INTERNATIONAL,
                newNumberFormats));

            newNumberFormats[0].NationalPrefixFormattingRule = "$NP$FG";
            newNumberFormats[0].Pattern ="(\\d{2}\\d{4})(\\d{4})";
            newNumberFormats[0].Format = "$1 $2 $3";
            Assert.Equal("020 7031 3000",
                phoneUtil.FormatByPattern(GBNumber, PhoneNumberFormat.NATIONAL,
                newNumberFormats));

            newNumberFormats[0].NationalPrefixFormattingRule = "($NP$FG)";
            Assert.Equal("(020) 7031 3000",
                phoneUtil.FormatByPattern(GBNumber, PhoneNumberFormat.NATIONAL,
                newNumberFormats));

            newNumberFormats[0].NationalPrefixFormattingRule = "";
            Assert.Equal("20 7031 3000",
                phoneUtil.FormatByPattern(GBNumber, PhoneNumberFormat.NATIONAL,
                newNumberFormats));

            Assert.Equal("+44 20 7031 3000",
                phoneUtil.FormatByPattern(GBNumber, PhoneNumberFormat.INTERNATIONAL,
                newNumberFormats));
        }

        [Fact]
        public void TestFormatE164Number()
        {
            Assert.Equal("+16502530000", phoneUtil.Format(USNumber, PhoneNumberFormat.E164));
            Assert.Equal("+4930123456", phoneUtil.Format(DENumber, PhoneNumberFormat.E164));
            Assert.Equal("+80012345678", phoneUtil.Format(InternationalTollFree, PhoneNumberFormat.E164));
        }

        [Fact]
        public void TestFormatNumberWithExtension()
        {
            var nzNumber = new PhoneNumber().MergeFrom(NZNumber);
            nzNumber.Extension = "1234";            // Uses default extension prefix:
            Assert.Equal("03-331 6005 ext. 1234", phoneUtil.Format(nzNumber, PhoneNumberFormat.NATIONAL));
            // Uses RFC 3966 syntax.
            Assert.Equal("tel:+64-3-331-6005;ext=1234", phoneUtil.Format(nzNumber, PhoneNumberFormat.RFC3966));
            // Extension prefix overridden in the territory information for the US:
            var usNumberWithExtension = new PhoneNumber().MergeFrom(USNumber);
            usNumberWithExtension.Extension = "4567";
            Assert.Equal("650 253 0000 extn. 4567", phoneUtil.Format(usNumberWithExtension,
                PhoneNumberFormat.NATIONAL));
        }

        [Fact]
        public void TestFormatInOriginalFormat()
        {
            var number1 = phoneUtil.ParseAndKeepRawInput("+442087654321", RegionCode.GB);
            Assert.Equal("+44 20 8765 4321", phoneUtil.FormatInOriginalFormat(number1, RegionCode.GB));

            var number2 = phoneUtil.ParseAndKeepRawInput("02087654321", RegionCode.GB);
            Assert.Equal("(020) 8765 4321", phoneUtil.FormatInOriginalFormat(number2, RegionCode.GB));

            var number3 = phoneUtil.ParseAndKeepRawInput("011442087654321", RegionCode.US);
            Assert.Equal("011 44 20 8765 4321", phoneUtil.FormatInOriginalFormat(number3, RegionCode.US));

            var number4 = phoneUtil.ParseAndKeepRawInput("442087654321", RegionCode.GB);
            Assert.Equal("44 20 8765 4321", phoneUtil.FormatInOriginalFormat(number4, RegionCode.GB));

            var number5 = phoneUtil.Parse("+442087654321", RegionCode.GB);
            Assert.Equal("(020) 8765 4321", phoneUtil.FormatInOriginalFormat(number5, RegionCode.GB));

            // Invalid numbers that we have a formatting pattern for should be formatted properly. Note area
            // codes starting with 7 are intentionally excluded in the test metadata for testing purposes.
            var number6 = phoneUtil.ParseAndKeepRawInput("7345678901", RegionCode.US);
            Assert.Equal("734 567 8901", phoneUtil.FormatInOriginalFormat(number6, RegionCode.US));

            // US is not a leading zero country, and the presence of the leading zero leads us to format the
            // number using raw_input.
            var number7 = phoneUtil.ParseAndKeepRawInput("0734567 8901", RegionCode.US);
            Assert.Equal("0734567 8901", phoneUtil.FormatInOriginalFormat(number7, RegionCode.US));

            // This number is valid, but we don't have a formatting pattern for it. Fall back to the raw
            // input.
            var number8 = phoneUtil.ParseAndKeepRawInput("02-4567-8900", RegionCode.KR);
            Assert.Equal("02-4567-8900", phoneUtil.FormatInOriginalFormat(number8, RegionCode.KR));

            var number9 = phoneUtil.ParseAndKeepRawInput("01180012345678", RegionCode.US);
            Assert.Equal("011 800 1234 5678", phoneUtil.FormatInOriginalFormat(number9, RegionCode.US));

            var number10 = phoneUtil.ParseAndKeepRawInput("+80012345678", RegionCode.KR);
            Assert.Equal("+800 1234 5678", phoneUtil.FormatInOriginalFormat(number10, RegionCode.KR));

            // US local numbers are formatted correctly, as we have formatting patterns for them.
            var localNumberUS = phoneUtil.ParseAndKeepRawInput("2530000", RegionCode.US);
            Assert.Equal("253 0000", phoneUtil.FormatInOriginalFormat(localNumberUS, RegionCode.US));

            var numberWithNationalPrefixUS =
            phoneUtil.ParseAndKeepRawInput("18003456789", RegionCode.US);
            Assert.Equal("1 800 345 6789",
            phoneUtil.FormatInOriginalFormat(numberWithNationalPrefixUS, RegionCode.US));

            var numberWithoutNationalPrefixGB =
            phoneUtil.ParseAndKeepRawInput("2087654321", RegionCode.GB);
            Assert.Equal("20 8765 4321",
            phoneUtil.FormatInOriginalFormat(numberWithoutNationalPrefixGB, RegionCode.GB));
            // Make sure no metadata is modified as a result of the previous function call.
            Assert.Equal("(020) 8765 4321", phoneUtil.FormatInOriginalFormat(number5, RegionCode.GB));

            var numberWithNationalPrefixMX =
            phoneUtil.ParseAndKeepRawInput("013312345678", RegionCode.MX);
            Assert.Equal("01 33 1234 5678",
            phoneUtil.FormatInOriginalFormat(numberWithNationalPrefixMX, RegionCode.MX));

            var numberWithoutNationalPrefixMX =
            phoneUtil.ParseAndKeepRawInput("3312345678", RegionCode.MX);
            Assert.Equal("33 1234 5678",
            phoneUtil.FormatInOriginalFormat(numberWithoutNationalPrefixMX, RegionCode.MX));

            var numberWithoutNationalPrefixJP =
            phoneUtil.ParseAndKeepRawInput("0777012", RegionCode.JP);
            Assert.Equal("0777012",
            phoneUtil.FormatInOriginalFormat(numberWithoutNationalPrefixJP, RegionCode.JP));

            var numberWithCarrierCodeBR =
            phoneUtil.ParseAndKeepRawInput("012 3121286979", RegionCode.BR);
            Assert.Equal("012 3121286979",
            phoneUtil.FormatInOriginalFormat(numberWithCarrierCodeBR, RegionCode.BR));

            // The default national prefix used in this case is 045. When a number with national prefix 044
            // is entered, we return the raw input as we don't want to change the number entered.
            var numberWithNationalPrefixMX1 =
            phoneUtil.ParseAndKeepRawInput("044(33)1234-5678", RegionCode.MX);
            Assert.Equal("044(33)1234-5678",
            phoneUtil.FormatInOriginalFormat(numberWithNationalPrefixMX1, RegionCode.MX));

            var numberWithNationalPrefixMX2 =
            phoneUtil.ParseAndKeepRawInput("045(33)1234-5678", RegionCode.MX);
            Assert.Equal("045 33 1234 5678",
            phoneUtil.FormatInOriginalFormat(numberWithNationalPrefixMX2, RegionCode.MX));

            // The default international prefix used in this case is 0011. When a number with international
            // prefix 0012 is entered, we return the raw input as we don't want to change the number
            // entered.
            var outOfCountryNumberFromAU1 =
            phoneUtil.ParseAndKeepRawInput("0012 16502530000", RegionCode.AU);
            Assert.Equal("0012 16502530000",
            phoneUtil.FormatInOriginalFormat(outOfCountryNumberFromAU1, RegionCode.AU));

            var outOfCountryNumberFromAU2 =
            phoneUtil.ParseAndKeepRawInput("0011 16502530000", RegionCode.AU);
            Assert.Equal("0011 1 650 253 0000",
            phoneUtil.FormatInOriginalFormat(outOfCountryNumberFromAU2, RegionCode.AU));

            // Test the star sign is not removed from or added to the original input by this method.
            var starNumber = phoneUtil.ParseAndKeepRawInput("*1234", RegionCode.JP);
            Assert.Equal("*1234", phoneUtil.FormatInOriginalFormat(starNumber, RegionCode.JP));
            var numberWithoutStar = phoneUtil.ParseAndKeepRawInput("1234", RegionCode.JP);
            Assert.Equal("1234", phoneUtil.FormatInOriginalFormat(numberWithoutStar, RegionCode.JP));
        }

        [Fact]
        public void TestIsPremiumRate()
        {
            Assert.Equal(PhoneNumberType.PREMIUM_RATE, phoneUtil.GetNumberType(USPremium));

            var premiumRateNumber = new PhoneNumber {CountryCode = 39, NationalNumber = 892123L};
            Assert.Equal(PhoneNumberType.PREMIUM_RATE,
                phoneUtil.GetNumberType(premiumRateNumber));

            premiumRateNumber = new PhoneNumber {CountryCode = 44, NationalNumber = 9187654321L};
            Assert.Equal(PhoneNumberType.PREMIUM_RATE,
                phoneUtil.GetNumberType(premiumRateNumber));

            premiumRateNumber = new PhoneNumber
            {
                CountryCode = 49,
                NationalNumber = 9001654321L
            };
            Assert.Equal(PhoneNumberType.PREMIUM_RATE,
                phoneUtil.GetNumberType(premiumRateNumber));

            premiumRateNumber = new PhoneNumber
            {
                CountryCode = 49,
                NationalNumber = 90091234567L
            };
            Assert.Equal(PhoneNumberType.PREMIUM_RATE,
                phoneUtil.GetNumberType(premiumRateNumber));

            Assert.Equal(PhoneNumberType.PREMIUM_RATE,
                 phoneUtil.GetNumberType(UniversalPremiumRate));
        }

        [Fact]
        public void TestIsTollFree()
        {
            var tollFreeNumber = new PhoneNumber {CountryCode = 1, NationalNumber = 8881234567L};
            Assert.Equal(PhoneNumberType.TOLL_FREE,
                phoneUtil.GetNumberType(tollFreeNumber));

            tollFreeNumber = new PhoneNumber
            {
                CountryCode = 39,
                NationalNumber = 803123L
            };
            Assert.Equal(PhoneNumberType.TOLL_FREE,
                phoneUtil.GetNumberType(tollFreeNumber));

            tollFreeNumber = new PhoneNumber
            {
                CountryCode = 44,
                NationalNumber = 8012345678L
            };
            Assert.Equal(PhoneNumberType.TOLL_FREE,
                phoneUtil.GetNumberType(tollFreeNumber));

            tollFreeNumber = new PhoneNumber
            {
                CountryCode = 49,
                NationalNumber = 8001234567L
            };
            Assert.Equal(PhoneNumberType.TOLL_FREE,
                phoneUtil.GetNumberType(tollFreeNumber));

            Assert.Equal(PhoneNumberType.TOLL_FREE,
                 phoneUtil.GetNumberType(InternationalTollFree));
        }

        [Fact]
        public void TestIsMobile()
        {
            Assert.Equal(PhoneNumberType.MOBILE, phoneUtil.GetNumberType(BSMobile));
            Assert.Equal(PhoneNumberType.MOBILE, phoneUtil.GetNumberType(GBMobile));
            Assert.Equal(PhoneNumberType.MOBILE, phoneUtil.GetNumberType(ITMobile));
            Assert.Equal(PhoneNumberType.MOBILE, phoneUtil.GetNumberType(ARMobile));

            var mobileNumber = new PhoneNumber {CountryCode = 49, NationalNumber = 15123456789L};
            Assert.Equal(PhoneNumberType.MOBILE, phoneUtil.GetNumberType(mobileNumber));
        }

        [Fact]
        public void TestIsFixedLine()
        {
            Assert.Equal(PhoneNumberType.FIXED_LINE, phoneUtil.GetNumberType(BSNumber));
            Assert.Equal(PhoneNumberType.FIXED_LINE, phoneUtil.GetNumberType(ITNumber));
            Assert.Equal(PhoneNumberType.FIXED_LINE, phoneUtil.GetNumberType(GBNumber));
            Assert.Equal(PhoneNumberType.FIXED_LINE, phoneUtil.GetNumberType(DENumber));
        }

        [Fact]
        public void TestIsFixedLineAndMobile()
        {
            Assert.Equal(PhoneNumberType.FIXED_LINE_OR_MOBILE,
                 phoneUtil.GetNumberType(USNumber));

            var fixedLineAndMobileNumber = new PhoneNumber {CountryCode = 54, NationalNumber = 1987654321L};
            Assert.Equal(PhoneNumberType.FIXED_LINE_OR_MOBILE,
                 phoneUtil.GetNumberType(fixedLineAndMobileNumber));
        }

        [Fact]
        public void TestIsSharedCost()
        {
            var gbNumber = new PhoneNumber {CountryCode = 44, NationalNumber = 8431231234L};
            Assert.Equal(PhoneNumberType.SHARED_COST, phoneUtil.GetNumberType(gbNumber));
        }

        [Fact]
        public void TestIsVoip()
        {
            var gbNumber = new PhoneNumber { CountryCode = 44, NationalNumber = 5631231234L };
            Assert.Equal(PhoneNumberType.VOIP, phoneUtil.GetNumberType(gbNumber));
        }

        [Fact]
        public void TestIsPersonalNumber()
        {
            var gbNumber = new PhoneNumber { CountryCode = 44, NationalNumber = 7031231234L };
            Assert.Equal(PhoneNumberType.PERSONAL_NUMBER,
                phoneUtil.GetNumberType(gbNumber));
        }

        [Fact]
        public void TestIsUnknown()
        {
            // Invalid numbers should be of type UNKNOWN.
            Assert.Equal(PhoneNumberType.UNKNOWN, phoneUtil.GetNumberType(USLocalNumber));
        }

        [Fact]
        public void TestIsValidNumber()
        {
            Assert.True(phoneUtil.IsValidNumber(USNumber));
            Assert.True(phoneUtil.IsValidNumber(ITNumber));
            Assert.True(phoneUtil.IsValidNumber(GBMobile));
            Assert.True(phoneUtil.IsValidNumber(InternationalTollFree));
            Assert.True(phoneUtil.IsValidNumber(UniversalPremiumRate));

            var nzNumber = new PhoneNumber {CountryCode = 64, NationalNumber = 21387835L };
            Assert.True(phoneUtil.IsValidNumber(nzNumber));
        }

        [Fact]
        public void TestIsValidForRegion()
        {
            // This number is valid for the Bahamas, but is not a valid US number.
            Assert.True(phoneUtil.IsValidNumber(BSNumber));
            Assert.True(phoneUtil.IsValidNumberForRegion(BSNumber, RegionCode.BS));
            Assert.False(phoneUtil.IsValidNumberForRegion(BSNumber, RegionCode.US));
            var bsInvalidNumber =
                new PhoneNumber {CountryCode =1, NationalNumber = 2421232345L };
            // This number is no longer valid.
            Assert.False(phoneUtil.IsValidNumber(bsInvalidNumber));
            // La Mayotte and Reunion use 'leadingDigits' to differentiate them.
            var reNumber = new PhoneNumber { CountryCode = 262, NationalNumber = 262123456L };
            Assert.True(phoneUtil.IsValidNumber(reNumber));
            Assert.True(phoneUtil.IsValidNumberForRegion(reNumber, RegionCode.RE));
            Assert.False(phoneUtil.IsValidNumberForRegion(reNumber, RegionCode.YT));
            // Now change the number to be a number for La Mayotte.
            reNumber.NationalNumber = 269601234L;
            Assert.True(phoneUtil.IsValidNumberForRegion(reNumber, RegionCode.YT));
            Assert.False(phoneUtil.IsValidNumberForRegion(reNumber, RegionCode.RE));
            // This number is no longer valid for La Reunion.
            reNumber.NationalNumber = 269123456L;
            Assert.False(phoneUtil.IsValidNumberForRegion(reNumber, RegionCode.YT));
            Assert.False(phoneUtil.IsValidNumberForRegion(reNumber, RegionCode.RE));
            Assert.False(phoneUtil.IsValidNumber(reNumber));
            // However, it should be recognised as from La Mayotte, since it is valid for this region.
            Assert.Equal(RegionCode.YT, phoneUtil.GetRegionCodeForNumber(reNumber));
            // This number is valid in both places.
            reNumber.NationalNumber = 800123456L;
            Assert.True(phoneUtil.IsValidNumberForRegion(reNumber, RegionCode.YT));
            Assert.True(phoneUtil.IsValidNumberForRegion(reNumber, RegionCode.RE));
            Assert.True(phoneUtil.IsValidNumberForRegion(InternationalTollFree, RegionCode.UN001));
            Assert.False(phoneUtil.IsValidNumberForRegion(InternationalTollFree, RegionCode.US));

            Assert.False(phoneUtil.IsValidNumberForRegion(InternationalTollFree, RegionCode.ZZ));

            // Invalid country calling codes.
            var invalidNumber = new PhoneNumber {CountryCode = 3923, NationalNumber = 2366L };
            Assert.False(phoneUtil.IsValidNumberForRegion(invalidNumber, RegionCode.ZZ));
            Assert.False(phoneUtil.IsValidNumberForRegion(invalidNumber, RegionCode.UN001));
            invalidNumber = new PhoneNumber {CountryCode = 0};
            Assert.False(phoneUtil.IsValidNumberForRegion(invalidNumber, RegionCode.UN001));
            Assert.False(phoneUtil.IsValidNumberForRegion(invalidNumber, RegionCode.ZZ));
        }

        [Fact]
        public void TestIsNotValidNumber()
        {
            Assert.False(phoneUtil.IsValidNumber(USLocalNumber));

            var invalidNumber = new PhoneNumber { CountryCode = 49, NationalNumber = 1234L };
            Assert.False(phoneUtil.IsValidNumber(invalidNumber));

            invalidNumber = new PhoneNumber { CountryCode = 64, NationalNumber = 3316005L };
            Assert.False(phoneUtil.IsValidNumber(invalidNumber));

            // Invalid country calling codes.
            invalidNumber = new PhoneNumber {CountryCode = 3923, NationalNumber = 2366L };
            Assert.False(phoneUtil.IsValidNumber(invalidNumber));
            invalidNumber = new PhoneNumber {CountryCode = 0, NationalNumber = 2366L };
            Assert.False(phoneUtil.IsValidNumber(invalidNumber));

            Assert.False(phoneUtil.IsValidNumber(InternationalTollFreeTooLong));
        }

        [Fact]
        public void TestGetRegionCodeForCountryCode()
        {
            Assert.Equal(RegionCode.US, phoneUtil.GetRegionCodeForCountryCode(1));
            Assert.Equal(RegionCode.GB, phoneUtil.GetRegionCodeForCountryCode(44));
            Assert.Equal(RegionCode.DE, phoneUtil.GetRegionCodeForCountryCode(49));
            Assert.Equal(RegionCode.UN001, phoneUtil.GetRegionCodeForCountryCode(800));
            Assert.Equal(RegionCode.UN001, phoneUtil.GetRegionCodeForCountryCode(979));
        }

        [Fact]
        public void TestGetRegionCodeForNumber()
        {
            Assert.Equal(RegionCode.BS, phoneUtil.GetRegionCodeForNumber(BSNumber));
            Assert.Equal(RegionCode.US, phoneUtil.GetRegionCodeForNumber(USNumber));
            Assert.Equal(RegionCode.GB, phoneUtil.GetRegionCodeForNumber(GBMobile));
            Assert.Equal(RegionCode.UN001, phoneUtil.GetRegionCodeForNumber(InternationalTollFree));
            Assert.Equal(RegionCode.UN001, phoneUtil.GetRegionCodeForNumber(UniversalPremiumRate));
        }

        [Fact]
        public void TestGetCountryCodeForRegion()
        {
            Assert.Equal(1, phoneUtil.GetCountryCodeForRegion(RegionCode.US));
            Assert.Equal(64, phoneUtil.GetCountryCodeForRegion(RegionCode.NZ));
            Assert.Equal(0, phoneUtil.GetCountryCodeForRegion(null));
            Assert.Equal(0, phoneUtil.GetCountryCodeForRegion(RegionCode.ZZ));
            Assert.Equal(0, phoneUtil.GetCountryCodeForRegion(RegionCode.UN001));
            // CS is already deprecated so the library doesn't support it.
            Assert.Equal(0, phoneUtil.GetCountryCodeForRegion(RegionCode.CS));
        }

        [Fact]
        public void TestGetNationalDiallingPrefixForRegion()
        {
            Assert.Equal("1", phoneUtil.GetNddPrefixForRegion(RegionCode.US, false));
            // Test non-main country to see it gets the national dialling prefix for the main country with
            // that country calling code.
            Assert.Equal("1", phoneUtil.GetNddPrefixForRegion(RegionCode.BS, false));
            Assert.Equal("0", phoneUtil.GetNddPrefixForRegion(RegionCode.NZ, false));
            // Test case with non digit in the national prefix.
            Assert.Equal("0~0", phoneUtil.GetNddPrefixForRegion(RegionCode.AO, false));
            Assert.Equal("00", phoneUtil.GetNddPrefixForRegion(RegionCode.AO, true));
            // Test cases with invalid regions.
            Assert.Equal(null, phoneUtil.GetNddPrefixForRegion(null, false));
            Assert.Equal(null, phoneUtil.GetNddPrefixForRegion(RegionCode.ZZ, false));
            Assert.Equal(null, phoneUtil.GetNddPrefixForRegion(RegionCode.UN001, false));
            // CS is already deprecated so the library doesn't support it.
            Assert.Equal(null, phoneUtil.GetNddPrefixForRegion(RegionCode.CS, false));
        }

        [Fact]
        public void TestIsNANPACountry()
        {
            Assert.True(phoneUtil.IsNANPACountry(RegionCode.US));
            Assert.True(phoneUtil.IsNANPACountry(RegionCode.BS));
            Assert.False(phoneUtil.IsNANPACountry(RegionCode.DE));
            Assert.False(phoneUtil.IsNANPACountry(RegionCode.ZZ));
            Assert.False(phoneUtil.IsNANPACountry(RegionCode.UN001));
            Assert.False(phoneUtil.IsNANPACountry(null));
        }

        [Fact]
        public void TestIsPossibleNumber()
        {
            Assert.True(phoneUtil.IsPossibleNumber(GBNumber));
            Assert.True(phoneUtil.IsPossibleNumber(InternationalTollFree));

            Assert.True(phoneUtil.IsPossibleNumber("+1 650 253 0000", RegionCode.US));
            Assert.True(phoneUtil.IsPossibleNumber("+1 650 GOO OGLE", RegionCode.US));
            Assert.True(phoneUtil.IsPossibleNumber("(650) 253-0000", RegionCode.US));
            Assert.True(phoneUtil.IsPossibleNumber("+1 650 253 0000", RegionCode.GB));
            Assert.True(phoneUtil.IsPossibleNumber("+44 20 7031 3000", RegionCode.GB));
            Assert.True(phoneUtil.IsPossibleNumber("(020) 7031 3000", RegionCode.GB));
            Assert.True(phoneUtil.IsPossibleNumber("3331 6005", RegionCode.NZ));
            Assert.True(phoneUtil.IsPossibleNumber("+800 1234 5678", RegionCode.UN001));
        }

        [Fact]
        public void TestIsPossibleNumberWithReason()
        {
            // National numbers for country calling code +1 that are within 7 to 10 digits are possible.
            Assert.Equal(PhoneNumberUtil.ValidationResult.IS_POSSIBLE,
            phoneUtil.IsPossibleNumberWithReason(USNumber));

            Assert.Equal(PhoneNumberUtil.ValidationResult.IS_POSSIBLE_LOCAL_ONLY,
            phoneUtil.IsPossibleNumberWithReason(USLocalNumber));

            Assert.Equal(PhoneNumberUtil.ValidationResult.TOO_LONG,
            phoneUtil.IsPossibleNumberWithReason(USLongNumber));

            var number = new PhoneNumber {CountryCode = 0, NationalNumber = 2530000L };
            Assert.Equal(PhoneNumberUtil.ValidationResult.INVALID_COUNTRY_CODE,
            phoneUtil.IsPossibleNumberWithReason(number));

            number = new PhoneNumber {CountryCode =1, NationalNumber = 253000L };
            Assert.Equal(PhoneNumberUtil.ValidationResult.TOO_SHORT,
            phoneUtil.IsPossibleNumberWithReason(number));

            number = new PhoneNumber {CountryCode = 65, NationalNumber = 1234567890L };
            Assert.Equal(PhoneNumberUtil.ValidationResult.IS_POSSIBLE,
            phoneUtil.IsPossibleNumberWithReason(number));

            Assert.Equal(PhoneNumberUtil.ValidationResult.TOO_LONG,
                 phoneUtil.IsPossibleNumberWithReason(InternationalTollFreeTooLong));

            // Try with number that we don't have metadata for.
            var adNumber = new PhoneNumber {CountryCode = 376, NationalNumber = 12345L };
            Assert.Equal(PhoneNumberUtil.ValidationResult.TOO_SHORT,
                phoneUtil.IsPossibleNumberWithReason(adNumber));
            adNumber = new PhoneNumber {CountryCode = 376, NationalNumber = 1L};
            Assert.Equal(PhoneNumberUtil.ValidationResult.TOO_SHORT,
                phoneUtil.IsPossibleNumberWithReason(adNumber));
            adNumber = new PhoneNumber {CountryCode = 376, NationalNumber = 12345678901234567L};
            Assert.Equal(PhoneNumberUtil.ValidationResult.TOO_LONG,
                phoneUtil.IsPossibleNumberWithReason(adNumber));
        }

        [Fact]
        public void TestIsNotPossibleNumber()
        {
            Assert.False(phoneUtil.IsPossibleNumber(USLongNumber));
            Assert.False(phoneUtil.IsPossibleNumber(InternationalTollFreeTooLong));

            var number = new PhoneNumber {CountryCode = 1, NationalNumber = 253000L};
            Assert.False(phoneUtil.IsPossibleNumber(number));

            number = new PhoneNumber { CountryCode = 44, NationalNumber = 300L };
            Assert.False(phoneUtil.IsPossibleNumber(number));

            Assert.False(phoneUtil.IsPossibleNumber("+1 650 253 00000", RegionCode.US));
            Assert.False(phoneUtil.IsPossibleNumber("(650) 253-00000", RegionCode.US));
            Assert.False(phoneUtil.IsPossibleNumber("I want a Pizza", RegionCode.US));
            Assert.False(phoneUtil.IsPossibleNumber("253-000", RegionCode.US));
            Assert.False(phoneUtil.IsPossibleNumber("1 3000", RegionCode.GB));
            Assert.False(phoneUtil.IsPossibleNumber("+44 300", RegionCode.GB));
            Assert.False(phoneUtil.IsPossibleNumber("+800 1234 5678 9", RegionCode.UN001));
        }

        [Fact]
        public void TestTruncateTooLongNumber()
        {
            // GB number 080 1234 5678, but entered with 4 extra digits at the end.
            var tooLongNumber = new PhoneNumber { CountryCode = 44, NationalNumber = 80123456780123L };
            var validNumber = new PhoneNumber { CountryCode = 44, NationalNumber = 8012345678L };
            Assert.True(phoneUtil.TruncateTooLongNumber(tooLongNumber));
            AreEqual(validNumber, tooLongNumber);

            // IT number 022 3456 7890, but entered with 3 extra digits at the end.
            tooLongNumber = new PhoneNumber { CountryCode = 39, NationalNumber = 2234567890123L, ItalianLeadingZero = true};
            validNumber = new PhoneNumber {CountryCode = 39, NationalNumber = 2234567890L, ItalianLeadingZero = true};
            Assert.True(phoneUtil.TruncateTooLongNumber(tooLongNumber));
            AreEqual(validNumber, tooLongNumber);

            // US number 650-253-0000, but entered with one additional digit at the end.
            tooLongNumber = new PhoneNumber()
                .MergeFrom(USLongNumber);
            Assert.True(phoneUtil.TruncateTooLongNumber(tooLongNumber));
            Assert.Equal(USNumber, tooLongNumber);

            tooLongNumber = new PhoneNumber()
                .MergeFrom(InternationalTollFreeTooLong);
            Assert.True(phoneUtil.TruncateTooLongNumber(tooLongNumber));
            Assert.Equal(InternationalTollFree, tooLongNumber);

            // Tests what happens when a valid number is passed in.
            var validNumberCopy = new PhoneNumber().MergeFrom(validNumber);
            Assert.True(phoneUtil.TruncateTooLongNumber(validNumber));
            // Tests the number is not modified.
            AreEqual(validNumberCopy, validNumber);

            // Tests what happens when a number with invalid prefix is passed in.
            // The test metadata says US numbers cannot have prefix 240.
            var numberWithInvalidPrefix = new PhoneNumber { CountryCode = 1, NationalNumber = 2401234567L};
            var invalidNumberCopy = new PhoneNumber().MergeFrom(numberWithInvalidPrefix);
            Assert.False(phoneUtil.TruncateTooLongNumber(numberWithInvalidPrefix));
            // Tests the number is not modified.
            AreEqual(invalidNumberCopy, numberWithInvalidPrefix);

            // Tests what happens when a too short number is passed in.
            var tooShortNumber = new PhoneNumber { CountryCode = 1, NationalNumber = 1234L };
            var tooShortNumberCopy = new PhoneNumber().MergeFrom(tooShortNumber);
            Assert.False(phoneUtil.TruncateTooLongNumber(tooShortNumber));
            // Tests the number is not modified.
            AreEqual(tooShortNumberCopy, tooShortNumber);
        }

        [Fact]
        public void TestIsViablePhoneNumber()
        {
            Assert.False(PhoneNumberUtil.IsViablePhoneNumber("1"));
            // Only one or two digits before strange non-possible punctuation.
            Assert.False(PhoneNumberUtil.IsViablePhoneNumber("1+1+1"));
            Assert.False(PhoneNumberUtil.IsViablePhoneNumber("80+0"));
             // Two digits is viable.
            Assert.True(PhoneNumberUtil.IsViablePhoneNumber("00"));
            Assert.True(PhoneNumberUtil.IsViablePhoneNumber("111"));
            // Alpha numbers.
            Assert.True(PhoneNumberUtil.IsViablePhoneNumber("0800-4-pizza"));
            Assert.True(PhoneNumberUtil.IsViablePhoneNumber("0800-4-PIZZA"));
            // We need at least three digits before any alpha characters.
            Assert.False(PhoneNumberUtil.IsViablePhoneNumber("08-PIZZA"));
            Assert.False(PhoneNumberUtil.IsViablePhoneNumber("8-PIZZA"));
            Assert.False(PhoneNumberUtil.IsViablePhoneNumber("12. March"));
        }

        [Fact]
        public void TestIsViablePhoneNumberNonAscii()
        {
            // Only one or two digits before possible punctuation followed by more digits.
            Assert.True(PhoneNumberUtil.IsViablePhoneNumber("1\u300034"));
            Assert.False(PhoneNumberUtil.IsViablePhoneNumber("1\u30003+4"));
            // Unicode variants of possible starting character and other allowed punctuation/digits.
            Assert.True(PhoneNumberUtil.IsViablePhoneNumber("\uFF081\uFF09\u30003456789"));
            // Testing a leading + is okay.
            Assert.True(PhoneNumberUtil.IsViablePhoneNumber("+1\uFF09\u30003456789"));
        }

        [Fact]
        public void TestExtractPossibleNumber()
        {
            // Removes preceding funky punctuation and letters but leaves the rest untouched.
            Assert.Equal("0800-345-600", PhoneNumberUtil.ExtractPossibleNumber("Tel:0800-345-600"));
            Assert.Equal("0800 FOR PIZZA", PhoneNumberUtil.ExtractPossibleNumber("Tel:0800 FOR PIZZA"));
            // Should not remove plus sign
            Assert.Equal("+800-345-600", PhoneNumberUtil.ExtractPossibleNumber("Tel:+800-345-600"));
            // Should recognise wide digits as possible start values.
            Assert.Equal("\uFF10\uFF12\uFF13",
            PhoneNumberUtil.ExtractPossibleNumber("\uFF10\uFF12\uFF13"));
            // Dashes are not possible start values and should be removed.
            Assert.Equal("\uFF11\uFF12\uFF13",
            PhoneNumberUtil.ExtractPossibleNumber("Num-\uFF11\uFF12\uFF13"));
            // If not possible number present, return empty string.
            Assert.Equal("", PhoneNumberUtil.ExtractPossibleNumber("Num-...."));
            // Leading brackets are stripped - these are not used when parsing.
            Assert.Equal("650) 253-0000", PhoneNumberUtil.ExtractPossibleNumber("(650) 253-0000"));

            // Trailing non-alpha-numeric characters should be removed.
            Assert.Equal("650) 253-0000", PhoneNumberUtil.ExtractPossibleNumber("(650) 253-0000..- .."));
            Assert.Equal("650) 253-0000", PhoneNumberUtil.ExtractPossibleNumber("(650) 253-0000."));
            // This case has a trailing RTL char.
            Assert.Equal("650) 253-0000", PhoneNumberUtil.ExtractPossibleNumber("(650) 253-0000\u200F"));
        }

        [Fact]
        public void TestMaybeStripNationalPrefix()
        {
            var metadata = new PhoneMetadata
            {
                NationalPrefixForParsing = "34",
                GeneralDesc = new PhoneNumberDesc {NationalNumberPattern = "\\d{4,8}"}
            };
            var numberToStrip = new StringBuilder("34356778");
            var strippedNumber = "356778";
            Assert.True(phoneUtil.MaybeStripNationalPrefixAndCarrierCode(numberToStrip, metadata, null));
            Assert.Equal(strippedNumber, numberToStrip.ToString());
            // Retry stripping - now the number should not start with the national prefix, so no more
            // stripping should occur.
            Assert.False(phoneUtil.MaybeStripNationalPrefixAndCarrierCode(numberToStrip, metadata, null));
            Assert.Equal(strippedNumber, numberToStrip.ToString());
            // Some countries have no national prefix. Repeat test with none specified.
            metadata.BuildPartial(;
;
            Assert.False(phoneUtil.MaybeStripNationalPrefixAndCarrierCode(numberToStrip, metadata, null));
            Assert.Equal(strippedNumber, numberToStrip.ToString());
            // If the resultant number doesn't match the national rule, it shouldn't be stripped.
            metadata.BuildPartial(;
;
            numberToStrip = new StringBuilder("3123");
            strippedNumber = "3123";
            Assert.False(phoneUtil.MaybeStripNationalPrefixAndCarrierCode(numberToStrip, metadata, null));
            Assert.Equal(strippedNumber, numberToStrip.ToString());
            // Test extracting carrier selection code.
            metadata.BuildPartial(;
;
            numberToStrip = new StringBuilder("08122123456");
            strippedNumber = "22123456";
            var carrierCode = new StringBuilder();
            Assert.True(phoneUtil.MaybeStripNationalPrefixAndCarrierCode(
                numberToStrip, metadata, carrierCode));
            Assert.Equal("81", carrierCode.ToString());
            Assert.Equal(strippedNumber, numberToStrip.ToString());
            // If there was a transform rule, check it was applied.
            // Note that a capturing group is present here.
            metadata.NationalPrefixForParsing = "0(\\d{2}").BuildPartial };
;
            numberToStrip = new StringBuilder("031123");
            var transformedNumber = "5315123";
            Assert.True(phoneUtil.MaybeStripNationalPrefixAndCarrierCode(numberToStrip, metadata, null));
            Assert.Equal(transformedNumber, numberToStrip.ToString());
        }

        [Fact]
        public void TestMaybeStripInternationalPrefix()
        {
            var internationalPrefix = "00[39]";
            var numberToStrip = new StringBuilder("0034567700-3898003");
            // Note the dash is removed as part of the normalization.
            var strippedNumber = new StringBuilder("45677003898003");
            Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromNumberWithIdd,
                phoneUtil.MaybeStripInternationalPrefixAndNormalize(numberToStrip,
                    internationalPrefix));
            Assert.Equal(strippedNumber.ToString(), numberToStrip.ToString());
            // Now the number no longer starts with an IDD prefix, so it should now report
            // FromDefaultCountry.
            Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromDefaultCountry,
                phoneUtil.MaybeStripInternationalPrefixAndNormalize(numberToStrip,
                    internationalPrefix));

            numberToStrip = new StringBuilder("00945677003898003");
            Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromNumberWithIdd,
                phoneUtil.MaybeStripInternationalPrefixAndNormalize(numberToStrip,
                    internationalPrefix));
            Assert.Equal(strippedNumber.ToString(), numberToStrip.ToString());
            // Test it works when the international prefix is broken up by spaces.
            numberToStrip = new StringBuilder("00 9 45677003898003");
            Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromNumberWithIdd,
                phoneUtil.MaybeStripInternationalPrefixAndNormalize(numberToStrip,
                    internationalPrefix));
            Assert.Equal(strippedNumber.ToString(), numberToStrip.ToString());
            // Now the number no longer starts with an IDD prefix, so it should now report
            // FromDefaultCountry.
            Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromDefaultCountry,
                phoneUtil.MaybeStripInternationalPrefixAndNormalize(numberToStrip,
                    internationalPrefix));

            // Test the + symbol is also recognised and stripped.
            numberToStrip = new StringBuilder("+45677003898003");
            strippedNumber = new StringBuilder("45677003898003");
            Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromNumberWithPlusSign,
            phoneUtil.MaybeStripInternationalPrefixAndNormalize(numberToStrip,
                                                         internationalPrefix));
            Assert.Equal(strippedNumber.ToString(), numberToStrip.ToString());

            // If the number afterwards is a zero, we should not strip this - no country calling code begins
            // with 0.
            numberToStrip = new StringBuilder("0090112-3123");
            strippedNumber = new StringBuilder("00901123123");
            Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromDefaultCountry,
            phoneUtil.MaybeStripInternationalPrefixAndNormalize(numberToStrip,
                                                         internationalPrefix));
            Assert.Equal(strippedNumber.ToString(), numberToStrip.ToString());
            // Here the 0 is separated by a space from the IDD.
            numberToStrip = new StringBuilder("009 0-112-3123");
            Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromDefaultCountry,
            phoneUtil.MaybeStripInternationalPrefixAndNormalize(numberToStrip,
                                                         internationalPrefix));
        }

        [Fact]
        public void TestMaybeExtractCountryCode = 
        {
            var number = new PhoneNumber();
            var metadata = phoneUtil.GetMetadataForRegion(RegionCode.US);
            // Note that for the US, the IDD is 011.
            try
            {
                var phoneNumber = "011112-3456789";
                var strippedNumber = "123456789";
                var countryCallingCode = 1;
                var numberToFill = new StringBuilder();
                Assert.Equal(countryCallingCode,
                    phoneUtil.MaybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number));
                Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromNumberWithIdd, number.CountryCodeSource);
                // Should strip and normalize national significant number.
                Assert.Equal(strippedNumber,
                    numberToFill.ToString());
            }
            catch (NumberParseException)
            {
                Assert.True(false);
            }
            number = new PhoneNumber();
            try
            {
                var phoneNumber = "+6423456789";
                var countryCallingCode = 64;
                var numberToFill = new StringBuilder();
                Assert.Equal(countryCallingCode,
                    phoneUtil.MaybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number));
                Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromNumberWithPlusSign, number.CountryCodeSource);
            }
            catch (NumberParseException)
            {
                Assert.True(false);
            }
            number = new PhoneNumber();
            try
            {
                var phoneNumber = "+80012345678";
                var countryCallingCode = 800;
                var numberToFill = new StringBuilder();
                Assert.Equal(countryCallingCode,
                   phoneUtil.MaybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number));
                Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromNumberWithPlusSign, number.CountryCodeSource);
            }
            catch (NumberParseException)
            {
                Assert.True(false);
            }
            number = new PhoneNumber();
            try
            {
                var phoneNumber = "2345-6789";
                var numberToFill = new StringBuilder();
                Assert.Equal(0,
                phoneUtil.MaybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number));
                Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromDefaultCountry, number.CountryCodeSource);
            }
            catch (NumberParseException)
            {
                Assert.True(false);
            }
            number = new PhoneNumber();
            try
            {
                var phoneNumber = "0119991123456789";
                var numberToFill = new StringBuilder();
                phoneUtil.MaybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected.
                Assert.Equal(ErrorType.INVALID_COUNTRY_CODE,
                e.ErrorType);
            }
            number = new PhoneNumber();
            try
            {
                var phoneNumber = "(1 610) 619 4466";
                var countryCallingCode = 1;
                var numberToFill = new StringBuilder();
                Assert.Equal(countryCallingCode,
                phoneUtil.MaybeExtractCountryCode(phoneNumber, metadata, numberToFill, true,
                number));
                Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromNumberWithoutPlusSign,
                number.CountryCodeSource);
            }
            catch (NumberParseException)
            {
                Assert.True(false);
            }
            number = new PhoneNumber();
            try
            {
                var phoneNumber = "(1 610) 619 4466";
                var countryCallingCode = 1;
                var numberToFill = new StringBuilder();
                Assert.Equal(countryCallingCode,
                phoneUtil.MaybeExtractCountryCode(phoneNumber, metadata, numberToFill, false,
                number));
                Assert.False(number.HasCountryCodeSource, "Should not contain CountryCodeSource.");
            }
            catch (NumberParseException)
            {
                Assert.True(false);
            }
            number = new PhoneNumber();
            try
            {
                var phoneNumber = "(1 610) 619 446";
                var numberToFill = new StringBuilder();
                Assert.Equal(0,
                phoneUtil.MaybeExtractCountryCode(phoneNumber, metadata, numberToFill, false,
                number));
                Assert.False(number.HasCountryCodeSource, "Should not contain CountryCodeSource.");
            }
            catch (NumberParseException)
            {
                Assert.True(false);
            }
            number = new PhoneNumber();
            try
            {
                var phoneNumber = "(1 610) 619";
                var numberToFill = new StringBuilder();
                Assert.Equal(0,
                phoneUtil.MaybeExtractCountryCode(phoneNumber, metadata, numberToFill, true,
                number));
                Assert.Equal(PhoneNumber.Types.CountryCodeSource.FromDefaultCountry, number.CountryCodeSource);
            }
            catch (NumberParseException)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void TestParseNationalNumber()
        {
            // National prefix attached.
            Assert.Equal(NZNumber, phoneUtil.Parse("033316005", RegionCode.NZ));
            Assert.Equal(NZNumber, phoneUtil.Parse("33316005", RegionCode.NZ));
            // National prefix attached and some formatting present.
            Assert.Equal(NZNumber, phoneUtil.Parse("03-331 6005", RegionCode.NZ));
            Assert.Equal(NZNumber, phoneUtil.Parse("03 331 6005", RegionCode.NZ));
            // Test parsing RFC3966 format with a phone context.
            Assert.Equal(NZNumber, phoneUtil.Parse("tel:03-331-6005;phone-context=+64", RegionCode.NZ));
            Assert.Equal(NZNumber, phoneUtil.Parse("tel:331-6005;phone-context=+64-3", RegionCode.NZ));
            Assert.Equal(NZNumber, phoneUtil.Parse("tel:331-6005;phone-context=+64-3", RegionCode.US));

            // Test parsing RFC3966 format with optional user-defined parameters. The parameters will appear
            // after the context if present.
            Assert.Equal(NZNumber, phoneUtil.Parse("tel:03-331-6005;phone-context=+64;a=%A1",
                RegionCode.NZ));
            // Test parsing RFC3966 with an ISDN subaddress.
            Assert.Equal(NZNumber, phoneUtil.Parse("tel:03-331-6005;isub=12345;phone-context=+64",
                RegionCode.NZ));
            Assert.Equal(NZNumber, phoneUtil.Parse("tel:+64-3-331-6005;isub=12345", RegionCode.NZ));

            // Testing international prefixes.
            // Should strip country calling code.
            Assert.Equal(NZNumber, phoneUtil.Parse("0064 3 331 6005", RegionCode.NZ));
            // Try again, but this time we have an international number with Region Code US. It should
            // recognise the country calling code and parse accordingly.
            Assert.Equal(NZNumber, phoneUtil.Parse("01164 3 331 6005", RegionCode.US));
            Assert.Equal(NZNumber, phoneUtil.Parse("+64 3 331 6005", RegionCode.US));

            // We should ignore the leading plus here, since it is not followed by a valid country code but
            // instead is followed by the IDD for the US.
            Assert.Equal(NZNumber, phoneUtil.Parse("+01164 3 331 6005", RegionCode.US));
            Assert.Equal(NZNumber, phoneUtil.Parse("+0064 3 331 6005", RegionCode.NZ));
            Assert.Equal(NZNumber, phoneUtil.Parse("+ 00 64 3 331 6005", RegionCode.NZ));

            Assert.Equal(USLocalNumber,
                phoneUtil.Parse("tel:253-0000;phone-context=www.google.com", RegionCode.US));
            Assert.Equal(USLocalNumber,
                phoneUtil.Parse("tel:253-0000;isub=12345;phone-context=www.google.com", RegionCode.US));
            // This is invalid because no "+" sign is present as part of phone-context. The phone context
            // is simply ignored in this case just as if it contains a domain.
            Assert.Equal(USLocalNumber,
                phoneUtil.Parse("tel:2530000;isub=12345;phone-context=1-650", RegionCode.US));
            Assert.Equal(USLocalNumber,
                phoneUtil.Parse("tel:2530000;isub=12345;phone-context=1234.com", RegionCode.US));

            var nzNumber = new PhoneNumber { CountryCode = 64, NationalNumber = 64123456L };
            Assert.Equal(nzNumber, phoneUtil.Parse("64(0)64123456", RegionCode.NZ));
            // Check that using a "/" is fine in a phone number.
            Assert.Equal(DENumber, phoneUtil.Parse("301/23456", RegionCode.DE));

            // Check it doesn't use the '1' as a country calling code when parsing if the phone number was
            // already possible.
            var usNumber = new PhoneNumber { CountryCode = 1, NationalNumber = (1234567890L }            Assert.Equal(usNumber, phoneUtil.Parse("123-456-7890", RegionCode.US));

            // Test star numbers. Although this is not strictly valid, we would like to make sure we can
            // parse the output we produce when formatting the number.
            Assert.Equal(JPStarNumber, phoneUtil.Parse("+81 *2345", RegionCode.JP));

            var shortNumber = new PhoneNumber { CountryCode = 64, NationalNumber = 12L };
            Assert.Equal(shortNumber, phoneUtil.Parse("12", RegionCode.NZ));

            // Test for short-code with leading zero for a country which has 0 as national prefix. Ensure
            // it's not interpreted as national prefix if the remaining number length is local-only in
            // terms of length. Example: In GB, length 6-7 are only possible local-only.
            shortNumber = new PhoneNumber { CountryCode = 44, NationalNumber = (123456 }
            .ItalianLeadingZero = true)            Assert.Equal(shortNumber, phoneUtil.Parse("0123456", RegionCode.GB));
        }

        [Fact]
        public void TestParseNumberWithAlphaCharacters()
        {
            // Test case with alpha characters.
            var tollfreeNumber = new PhoneNumber { CountryCode = 64, NationalNumber = 800332005L };
            Assert.Equal(tollfreeNumber, phoneUtil.Parse("0800 DDA 005", RegionCode.NZ));
            var premiumNumber = new PhoneNumber { CountryCode = 64, NationalNumber = 9003326005L };
            Assert.Equal(premiumNumber, phoneUtil.Parse("0900 DDA 6005", RegionCode.NZ));
            // Not enough alpha characters for them to be considered intentional, so they are stripped.
            Assert.Equal(premiumNumber, phoneUtil.Parse("0900 332 6005a", RegionCode.NZ));
            Assert.Equal(premiumNumber, phoneUtil.Parse("0900 332 600a5", RegionCode.NZ));
            Assert.Equal(premiumNumber, phoneUtil.Parse("0900 332 600A5", RegionCode.NZ));
            Assert.Equal(premiumNumber, phoneUtil.Parse("0900 a332 600A5", RegionCode.NZ));
        }

        [Fact]
        public void TestParseMaliciousInput()
        {
            // Lots of leading + signs before the possible number.
            var maliciousNumber = new StringBuilder(6000);
            for(var i = 0; i < 6000; i++)
                maliciousNumber.Append('+');
            maliciousNumber.Append("12222-33-244 extensioB 343+");
            try
            {
                phoneUtil.Parse(maliciousNumber.ToString(), RegionCode.US);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.TOO_LONG,
                           e.ErrorType);
            }
            var maliciousNumberWithAlmostExt = new StringBuilder(6000);
            for(var i = 0; i < 350; i++)
                maliciousNumberWithAlmostExt.Append("200");
            maliciousNumberWithAlmostExt.Append(" extensiOB 345");
            try
            {
                phoneUtil.Parse(maliciousNumberWithAlmostExt.ToString(), RegionCode.US);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.TOO_LONG,
                           e.ErrorType);
            }
        }

        [Fact]
        public void TestParseWithInternationalPrefixes()
        {
            Assert.Equal(USNumber, phoneUtil.Parse("+1 (650) 253-0000", RegionCode.NZ));
            Assert.Equal(InternationalTollFree, phoneUtil.Parse("011 800 1234 5678", RegionCode.US));
            Assert.Equal(USNumber, phoneUtil.Parse("1-650-253-0000", RegionCode.US));
            // Calling the US number from Singapore by using different service providers
            // 1st test: calling using SingTel IDD service (IDD is 001)
            Assert.Equal(USNumber, phoneUtil.Parse("0011-650-253-0000", RegionCode.SG));
            // 2nd test: calling using StarHub IDD service (IDD is 008)
            Assert.Equal(USNumber, phoneUtil.Parse("0081-650-253-0000", RegionCode.SG));
            // 3rd test: calling using SingTel V019 service (IDD is 019)
            Assert.Equal(USNumber, phoneUtil.Parse("0191-650-253-0000", RegionCode.SG));
            // Calling the US number from Poland
            Assert.Equal(USNumber, phoneUtil.Parse("0~01-650-253-0000", RegionCode.PL));
            // Using "++" at the start.
            Assert.Equal(USNumber, phoneUtil.Parse("++1 (650) 253-0000", RegionCode.PL));
            // Using a very strange decimal digit range (Mongolian digits).
            Assert.Equal(USNumber, phoneUtil.Parse("\u1811 \u1816\u1815\u1810 " +
                "\u1812\u1815\u1813 \u1810\u1810\u1810\u1810",
                RegionCode.US));
        }

        [Fact]
        public void TestParseNonAscii()
        {
            // Using a full-width plus sign.
            Assert.Equal(USNumber, phoneUtil.Parse("\uFF0B1 (650) 253-0000", RegionCode.SG));
            // Using a soft hyphen U+00AD.
            Assert.Equal(USNumber, phoneUtil.Parse("1 (650) 253\u00AD-0000", RegionCode.US));
            // The whole number, including punctuation, is here represented in full-width form.
            Assert.Equal(USNumber, phoneUtil.Parse("\uFF0B\uFF11\u3000\uFF08\uFF16\uFF15\uFF10\uFF09" +
                                                "\u3000\uFF12\uFF15\uFF13\uFF0D\uFF10\uFF10\uFF10" +
                                                "\uFF10",
                                                RegionCode.SG));
            // Using U+30FC dash instead.
            Assert.Equal(USNumber, phoneUtil.Parse("\uFF0B\uFF11\u3000\uFF08\uFF16\uFF15\uFF10\uFF09" +
                                                "\u3000\uFF12\uFF15\uFF13\u30FC\uFF10\uFF10\uFF10" +
                                                "\uFF10",
                                                RegionCode.SG));
        }

        [Fact]
        public void TestParseWithLeadingZero()
        {
            Assert.Equal(ITNumber, phoneUtil.Parse("+39 02-36618 300", RegionCode.NZ));
            Assert.Equal(ITNumber, phoneUtil.Parse("02-36618 300", RegionCode.IT));

            Assert.Equal(ITMobile, phoneUtil.Parse("345 678 901", RegionCode.IT));
        }

        [Fact]
        public void TestParseNationalNumberArgentina()
        {
            // Test parsing mobile numbers of Argentina.
            var arNumber = new PhoneNumber { CountryCode = 54, NationalNumber = 93435551212L };
            Assert.Equal(arNumber, phoneUtil.Parse("+54 9 343 555 1212", RegionCode.AR));
            Assert.Equal(arNumber, phoneUtil.Parse("0343 15 555 1212", RegionCode.AR));

            arNumber = new PhoneNumber { CountryCode = 54, NationalNumber = 93715654320L };
            Assert.Equal(arNumber, phoneUtil.Parse("+54 9 3715 65 4320", RegionCode.AR));
            Assert.Equal(arNumber, phoneUtil.Parse("03715 15 65 4320", RegionCode.AR));
            Assert.Equal(ARMobile, phoneUtil.Parse("911 876 54321", RegionCode.AR));

            // Test parsing fixed-line numbers of Argentina.
            Assert.Equal(ARNumber, phoneUtil.Parse("+54 11 8765 4321", RegionCode.AR));
            Assert.Equal(ARNumber, phoneUtil.Parse("011 8765 4321", RegionCode.AR));

            arNumber = new PhoneNumber { CountryCode = 54, NationalNumber = 3715654321L };
            Assert.Equal(arNumber, phoneUtil.Parse("+54 3715 65 4321", RegionCode.AR));
            Assert.Equal(arNumber, phoneUtil.Parse("03715 65 4321", RegionCode.AR));

            arNumber = new PhoneNumber { CountryCode = 54, NationalNumber = 2312340000L };
            Assert.Equal(arNumber, phoneUtil.Parse("+54 23 1234 0000", RegionCode.AR));
            Assert.Equal(arNumber, phoneUtil.Parse("023 1234 0000", RegionCode.AR));
        }

        [Fact]
        public void TestParseWithXInNumber()
        {
            // Test that having an 'x' in the phone number at the start is ok and that it just gets removed.
            Assert.Equal(ARNumber, phoneUtil.Parse("01187654321", RegionCode.AR));
            Assert.Equal(ARNumber, phoneUtil.Parse("(0) 1187654321", RegionCode.AR));
            Assert.Equal(ARNumber, phoneUtil.Parse("0 1187654321", RegionCode.AR));
            Assert.Equal(ARNumber, phoneUtil.Parse("(0xx) 1187654321", RegionCode.AR));
            var arFromUs = new PhoneNumber { CountryCode = 54, NationalNumber = 81429712L };            // This test is intentionally constructed such that the number of digit after xx is larger than
            // 7, so that the number won't be mistakenly treated as an extension, as we allow extensions up
            // to 7 digits. This assumption is okay for now as all the countries where a carrier selection
            // code is written in the form of xx have a national significant number of length larger than 7.
            Assert.Equal(arFromUs, phoneUtil.Parse("011xx5481429712", RegionCode.US));
        }

        [Fact]
        public void TestParseNumbersMexico()
        {
            // Test parsing fixed-line numbers of Mexico.
            var mxNumber = new PhoneNumber { CountryCode = 52, NationalNumber = 4499780001L };
            Assert.Equal(mxNumber, phoneUtil.Parse("+52 (449)978-0001", RegionCode.MX));
            Assert.Equal(mxNumber, phoneUtil.Parse("01 (449)978-0001", RegionCode.MX));
            Assert.Equal(mxNumber, phoneUtil.Parse("(449)978-0001", RegionCode.MX));

            // Test parsing mobile numbers of Mexico.
            mxNumber = new PhoneNumber { CountryCode = 52, NationalNumber = 13312345678L };
            Assert.Equal(mxNumber, phoneUtil.Parse("+52 1 33 1234-5678", RegionCode.MX));
            Assert.Equal(mxNumber, phoneUtil.Parse("044 (33) 1234-5678", RegionCode.MX));
            Assert.Equal(mxNumber, phoneUtil.Parse("045 33 1234-5678", RegionCode.MX));
        }

        [Fact]
        public void TestFailedParseOnInvalidNumbers()
        {
            try
            {
                var sentencePhoneNumber = "This is not a phone number";
                phoneUtil.Parse(sentencePhoneNumber, RegionCode.NZ);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.NOT_A_NUMBER, e.ErrorType);
            }
            try
            {
                var sentencePhoneNumber = "1 Still not a number";
                phoneUtil.Parse(sentencePhoneNumber, RegionCode.NZ);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.NOT_A_NUMBER, e.ErrorType);
            }
            try
            {
                var sentencePhoneNumber = "1 MICROSOFT";
                phoneUtil.Parse(sentencePhoneNumber, RegionCode.NZ);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.NOT_A_NUMBER, e.ErrorType);
            }
            try
            {
                var sentencePhoneNumber = "12 MICROSOFT";
                phoneUtil.Parse(sentencePhoneNumber, RegionCode.NZ);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.NOT_A_NUMBER, e.ErrorType);
            }
            try
            {
                var tooLongPhoneNumber = "01495 72553301873 810104";
                phoneUtil.Parse(tooLongPhoneNumber, RegionCode.GB);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.TOO_LONG, e.ErrorType);
            }
            try
            {
                var plusMinusPhoneNumber = "+---";
                phoneUtil.Parse(plusMinusPhoneNumber, RegionCode.DE);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.NOT_A_NUMBER, e.ErrorType);
            }
            try
            {
                var plusStar = "+***";
                phoneUtil.Parse(plusStar, RegionCode.DE);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.NOT_A_NUMBER, e.ErrorType);
            }
            try
            {
                var plusStarPhoneNumber = "+*******91";
                phoneUtil.Parse(plusStarPhoneNumber, RegionCode.DE);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.NOT_A_NUMBER, e.ErrorType);
            }
            try
            {
                var tooShortPhoneNumber = "+49 0";
                phoneUtil.Parse(tooShortPhoneNumber, RegionCode.DE);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.TOO_SHORT_NSN, e.ErrorType);
            }
            try
            {
                var invalidCountryCode = "+210 3456 56789";
                phoneUtil.Parse(invalidCountryCode, RegionCode.NZ);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.INVALID_COUNTRY_CODE, e.ErrorType);
            }
            try
            {
                var plusAndIddAndInvalidCountryCode = "+ 00 210 3 331 6005";
                phoneUtil.Parse(plusAndIddAndInvalidCountryCode, RegionCode.NZ);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception. 00 is a correct IDD, but 210 is not a valid country code.
                Assert.Equal(ErrorType.INVALID_COUNTRY_CODE, e.ErrorType);
            }
            try
            {
                var someNumber = "123 456 7890";
                phoneUtil.Parse(someNumber, RegionCode.ZZ);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.INVALID_COUNTRY_CODE, e.ErrorType);
            }
            try
            {
                var someNumber = "123 456 7890";
                phoneUtil.Parse(someNumber, RegionCode.CS);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.INVALID_COUNTRY_CODE, e.ErrorType);
            }
            try
            {
                var someNumber = "123 456 7890";
                phoneUtil.Parse(someNumber, null);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.INVALID_COUNTRY_CODE, e.ErrorType);
            }
            try
            {
                var someNumber = "0044------";
                phoneUtil.Parse(someNumber, RegionCode.GB);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.TOO_SHORT_AFTER_IDD, e.ErrorType);
            }
            try
            {
                var someNumber = "0044";
                phoneUtil.Parse(someNumber, RegionCode.GB);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.TOO_SHORT_AFTER_IDD, e.ErrorType);
            }
            try
            {
                var someNumber = "011";
                phoneUtil.Parse(someNumber, RegionCode.US);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.TOO_SHORT_AFTER_IDD, e.ErrorType);
            }
            try
            {
                var someNumber = "0119";
                phoneUtil.Parse(someNumber, RegionCode.US);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.TOO_SHORT_AFTER_IDD, e.ErrorType);
            }
            try
            {
                var emptyNumber = "";
                // Invalid region.
                phoneUtil.Parse(emptyNumber, RegionCode.ZZ);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.NOT_A_NUMBER, e.ErrorType);
            }
            try
            {
                // Invalid region.
                phoneUtil.Parse(null, RegionCode.ZZ);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.NOT_A_NUMBER, e.ErrorType);
            }
            catch (ArgumentNullException)
            {
                Assert.True(false);
            }
            try
            {
                phoneUtil.Parse(null, RegionCode.US);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.NOT_A_NUMBER, e.ErrorType);
            }
            catch (ArgumentNullException)
            {
                Assert.True(false);
            }
            try
            {
                var domainRfcPhoneContext = "tel:555-1234;phone-context=www.google.com";
                phoneUtil.Parse(domainRfcPhoneContext, RegionCode.ZZ);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.INVALID_COUNTRY_CODE, e.ErrorType);
            }
            catch (ArgumentNullException)
            {
                Assert.True(false);
            }
            try
            {
                // This is invalid because no "+" sign is present as part of phone-context. This should not
                // succeed in being parsed.
                var invalidRfcPhoneContext = "tel:555-1234;phone-context=1-331";
                phoneUtil.Parse(invalidRfcPhoneContext, RegionCode.ZZ);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.INVALID_COUNTRY_CODE, e.ErrorType);
            }
        }

        [Fact]
        public void TestParseNumbersWithPlusWithNoRegion()
        {
            // RegionCode.ZZ is allowed only if the number starts with a '+' - then the country calling code
            // can be calculated.
            Assert.Equal(NZNumber, phoneUtil.Parse("+64 3 331 6005", RegionCode.ZZ));
            // Test with full-width plus.
            Assert.Equal(NZNumber, phoneUtil.Parse("\uFF0B64 3 331 6005", RegionCode.ZZ));
            // Test with normal plus but leading characters that need to be stripped.
            Assert.Equal(NZNumber, phoneUtil.Parse("Tel: +64 3 331 6005", RegionCode.ZZ));
            Assert.Equal(NZNumber, phoneUtil.Parse("+64 3 331 6005", null));
            Assert.Equal(InternationalTollFree, phoneUtil.Parse("+800 1234 5678", null));
            Assert.Equal(UniversalPremiumRate, phoneUtil.Parse("+979 123 456 789", null));

            // Test parsing RFC3966 format with a phone context.
            Assert.Equal(NZNumber, phoneUtil.Parse("tel:03-331-6005;phone-context=+64", RegionCode.ZZ));
            Assert.Equal(NZNumber, phoneUtil.Parse("  tel:03-331-6005;phone-context=+64", RegionCode.ZZ));
            Assert.Equal(NZNumber, phoneUtil.Parse("tel:03-331-6005;isub=12345;phone-context=+64",
                RegionCode.ZZ));

            var nzNumberWithRawInput = new PhoneNumber().MergeFrom(NZNumber)
                .RawInput = "+64 3 331 6005")
                .SetCountryCodeSource(PhoneNumber.Types.CountryCodeSource.FromNumberWithPlusSign)
            Assert.Equal(nzNumberWithRawInput, phoneUtil.ParseAndKeepRawInput("+64 3 331 6005",
                                                                      RegionCode.ZZ));
            // Null is also allowed for the region code in these cases.
            Assert.Equal(nzNumberWithRawInput, phoneUtil.ParseAndKeepRawInput("+64 3 331 6005", null));
        }

        [Fact]
        public void TestParseExtensions()
        {
            var nzNumber = new PhoneNumber { CountryCode = 64, NationalNumber = 33316005L, Extension = ("3456" }            Assert.Equal(nzNumber, phoneUtil.Parse("03 331 6005 ext 3456", RegionCode.NZ));
            Assert.Equal(nzNumber, phoneUtil.Parse("03-3316005x3456", RegionCode.NZ));
            Assert.Equal(nzNumber, phoneUtil.Parse("03-3316005 int.3456", RegionCode.NZ));
            Assert.Equal(nzNumber, phoneUtil.Parse("03 3316005 #3456", RegionCode.NZ));
            // Test the following do not extract extensions:
            Assert.Equal(AlphaNumericNumber, phoneUtil.Parse("1800 six-flags", RegionCode.US));
            Assert.Equal(AlphaNumericNumber, phoneUtil.Parse("1800 SIX FLAGS", RegionCode.US));
            Assert.Equal(AlphaNumericNumber, phoneUtil.Parse("0~0 1800 7493 5247", RegionCode.PL));
            Assert.Equal(AlphaNumericNumber, phoneUtil.Parse("(1800) 7493.5247", RegionCode.US));
            // Check that the last instance of an extension token is matched.
            var extnNumber = new PhoneNumber().MergeFrom(AlphaNumericNumber).Extension = "1234")            Assert.Equal(extnNumber, phoneUtil.Parse("0~0 1800 7493 5247 ~1234", RegionCode.PL));
            // Verifying fix where the last digit of a number was previously omitted if it was a 0 when
            // extracting the extension. Also verifying a few different cases of extensions.
            var ukNumber = new PhoneNumber { CountryCode = 44, NationalNumber = 2034567890L, Extension = ("456" }            Assert.Equal(ukNumber, phoneUtil.Parse("+44 2034567890x456", RegionCode.NZ));
            Assert.Equal(ukNumber, phoneUtil.Parse("+44 2034567890x456", RegionCode.GB));
            Assert.Equal(ukNumber, phoneUtil.Parse("+44 2034567890 x456", RegionCode.GB));
            Assert.Equal(ukNumber, phoneUtil.Parse("+44 2034567890 X456", RegionCode.GB));
            Assert.Equal(ukNumber, phoneUtil.Parse("+44 2034567890 X 456", RegionCode.GB));
            Assert.Equal(ukNumber, phoneUtil.Parse("+44 2034567890 X  456", RegionCode.GB));
            Assert.Equal(ukNumber, phoneUtil.Parse("+44 2034567890 x 456  ", RegionCode.GB));
            Assert.Equal(ukNumber, phoneUtil.Parse("+44 2034567890  X 456", RegionCode.GB));
            Assert.Equal(ukNumber, phoneUtil.Parse("+44-2034567890;ext=456", RegionCode.GB));
            Assert.Equal(ukNumber, phoneUtil.Parse("tel:2034567890;ext=456;phone-context=+44",
                                           RegionCode.ZZ));
            // Full-width extension, "extn" only.
            Assert.Equal(ukNumber, phoneUtil.Parse("+442034567890\uFF45\uFF58\uFF54\uFF4E456",
                RegionCode.GB));
            // "xtn" only.
            Assert.Equal(ukNumber, phoneUtil.Parse("+442034567890\uFF58\uFF54\uFF4E456",
                RegionCode.GB));
            // "xt" only.
            Assert.Equal(ukNumber, phoneUtil.Parse("+442034567890\uFF58\uFF54456",
                RegionCode.GB));

            var usWithExtension = new PhoneNumber { CountryCode = 1, NationalNumber = 8009013355L, Extension = ("7246433" }            Assert.Equal(usWithExtension, phoneUtil.Parse("(800) 901-3355 x 7246433", RegionCode.US));
            Assert.Equal(usWithExtension, phoneUtil.Parse("(800) 901-3355 , ext 7246433", RegionCode.US));
            Assert.Equal(usWithExtension,
                     phoneUtil.Parse("(800) 901-3355 ,extension 7246433", RegionCode.US));
            Assert.Equal(usWithExtension,
                     phoneUtil.Parse("(800) 901-3355 ,extensi\u00F3n 7246433", RegionCode.US));
            // Repeat with the small letter o with acute accent created by combining characters.
            Assert.Equal(usWithExtension,
                     phoneUtil.Parse("(800) 901-3355 ,extensio\u0301n 7246433", RegionCode.US));
            Assert.Equal(usWithExtension, phoneUtil.Parse("(800) 901-3355 , 7246433", RegionCode.US));
            Assert.Equal(usWithExtension, phoneUtil.Parse("(800) 901-3355 ext: 7246433", RegionCode.US));
            // Testing Russian extension \u0434\u043E\u0431 with variants found online.
            var ruWithExtension = new PhoneNumber
            {
                CountryCode = 7,
                NationalNumber = 4232022511L,
                Extension = "100"
            }.Build();
            Assert.Equal(ruWithExtension,
                phoneUtil.Parse("8 (423) 202-25-11, \u0434\u043E\u0431. 100", RegionCode.RU));
            Assert.Equal(ruWithExtension,
                phoneUtil.Parse("8 (423) 202-25-11 \u0434\u043E\u0431. 100", RegionCode.RU));
            Assert.Equal(ruWithExtension,
                phoneUtil.Parse("8 (423) 202-25-11, \u0434\u043E\u0431 100", RegionCode.RU));
            Assert.Equal(ruWithExtension,
                phoneUtil.Parse("8 (423) 202-25-11 \u0434\u043E\u0431 100", RegionCode.RU));
            Assert.Equal(ruWithExtension,
                phoneUtil.Parse("8 (423) 202-25-11\u0434\u043E\u0431100", RegionCode.RU));
            // In upper case
            Assert.Equal(ruWithExtension,
                phoneUtil.Parse("8 (423) 202-25-11, \u0414\u041E\u0411. 100", RegionCode.RU));

            // Test that if a number has two extensions specified, we ignore the second.
            var usWithTwoExtensionsNumber = new PhoneNumber { CountryCode = 1, NationalNumber = 2121231234L, Extension = ("508" }            Assert.Equal(usWithTwoExtensionsNumber, phoneUtil.Parse("(212)123-1234 x508/x1234",
                                                                RegionCode.US));
            Assert.Equal(usWithTwoExtensionsNumber, phoneUtil.Parse("(212)123-1234 x508/ x1234",
                                                                RegionCode.US));
            Assert.Equal(usWithTwoExtensionsNumber, phoneUtil.Parse("(212)123-1234 x508\\x1234",
                                                                RegionCode.US));

            // Test parsing numbers in the form (645) 123-1234-910# works, where the last 3 digits before
            // the # are an extension.
            usWithExtension = new PhoneNumber { CountryCode = 1, NationalNumber = 6451231234L, Extension = ("910" }            Assert.Equal(usWithExtension, phoneUtil.Parse("+1 (645) 123 1234-910#", RegionCode.US));
            // Retry with the same number in a slightly different format.
            Assert.Equal(usWithExtension, phoneUtil.Parse("+1 (645) 123 1234 ext. 910#", RegionCode.US));
        }

        [Fact]
        public void TestParseAndKeepRaw()
        {
            var alphaNumericNumber = new PhoneNumber().MergeFrom(AlphaNumericNumber)
                .RawInput = "800 six-flags")
                .SetCountryCodeSource(PhoneNumber.Types.CountryCodeSource.FromDefaultCountry)
            Assert.Equal(alphaNumericNumber,
                phoneUtil.ParseAndKeepRawInput("800 six-flags", RegionCode.US));

            var shorterAlphaNumber = new PhoneNumber { CountryCode = 1, NationalNumber = 8007493524L
                , RawInput = "1800 six-flag"
                .SetCountryCodeSource(PhoneNumber.Types.CountryCodeSource.FromNumberWithoutPlusSign)
            Assert.Equal(shorterAlphaNumber,
                phoneUtil.ParseAndKeepRawInput("1800 six-flag", RegionCode.US));

            shorterAlphaNumber.
;
                SetCountryCodeSource(PhoneNumber.Types.CountryCodeSource.FromNumberWithPlusSign)            Assert.Equal(shorterAlphaNumber,
                phoneUtil.ParseAndKeepRawInput("+1800 six-flag", RegionCode.NZ));

            shorterAlphaNumber.
;
                SetCountryCodeSource(PhoneNumber.Types.CountryCodeSource.FromNumberWithIdd)            Assert.Equal(shorterAlphaNumber,
                phoneUtil.ParseAndKeepRawInput("001800 six-flag", RegionCode.NZ));

            // Invalid region code supplied.
            try
            {
                phoneUtil.ParseAndKeepRawInput("123 456 7890", RegionCode.CS);
                Assert.True(false);
            }
            catch (NumberParseException e)
            {
                // Expected this exception.
                Assert.Equal(ErrorType.INVALID_COUNTRY_CODE, e.ErrorType);
            }

            var koreanNumber = new PhoneNumber { CountryCode = 82, NationalNumber = 22123456, RawInput = "08122123456", CountryCodeSource = (PhoneNumber.Types.CountryCodeSource.FromDefaultCountry }.
                SetPreferredDomesticCarrierCode("81")            Assert.Equal(koreanNumber, phoneUtil.ParseAndKeepRawInput("08122123456", RegionCode.KR));
        }

        [Fact]
        public void TestCountryWithNoNumberDesc()
        {
            // Andorra is a country where we don't have PhoneNumberDesc info in the metadata.
            var adNumber = new PhoneNumber { CountryCode = 376, NationalNumber = 12345L };
            Assert.Equal("+376 12345", phoneUtil.Format(adNumber, PhoneNumberFormat.INTERNATIONAL));
            Assert.Equal("+37612345", phoneUtil.Format(adNumber, PhoneNumberFormat.E164));
            Assert.Equal("12345", phoneUtil.Format(adNumber, PhoneNumberFormat.NATIONAL));
            Assert.Equal(PhoneNumberType.UNKNOWN, phoneUtil.GetNumberType(adNumber));

            // Test dialing a US number from within Andorra.
            Assert.Equal("00 1 650 253 0000",
            phoneUtil.FormatOutOfCountryCallingNumber(USNumber, RegionCode.AD));
        }

        [Fact]
        public void TestUnknownCountryCallingCodeForValidation()
        {
            var invalidNumber = new PhoneNumber { CountryCode = 0, NationalNumber = 1234L };
            Assert.False(phoneUtil.IsValidNumber(invalidNumber));
        }

        [Fact]
        public void TestIsNumberMatchMatches()
        {
            // Test simple matches where formatting is different, or leading zeroes, or country calling code
            // has been specified.
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                phoneUtil.IsNumberMatch("+64 3 331 6005", "+64 03 331 6005"));
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                 phoneUtil.IsNumberMatch("+800 1234 5678", "+80012345678"));
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                phoneUtil.IsNumberMatch("+64 03 331-6005", "+64 03331 6005"));
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                phoneUtil.IsNumberMatch("+643 331-6005", "+64033316005"));
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                phoneUtil.IsNumberMatch("+643 331-6005", "+6433316005"));
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                phoneUtil.IsNumberMatch("+64 3 331-6005", "+6433316005"));
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                 phoneUtil.IsNumberMatch("+64 3 331-6005", "tel:+64-3-331-6005;isub=123"));
            // Test alpha numbers.
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                phoneUtil.IsNumberMatch("+1800 siX-Flags", "+1 800 7493 5247"));
            // Test numbers with extensions.
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                phoneUtil.IsNumberMatch("+64 3 331-6005 extn 1234", "+6433316005#1234"));
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                phoneUtil.IsNumberMatch("+7 423 202-25-11 ext 100",
                    "+7 4232022511 \u0434\u043E\u0431. 100"));
            // Test proto buffers.
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                phoneUtil.IsNumberMatch(NZNumber, "+6403 331 6005"));

            var nzNumber = new PhoneNumber().MergeFrom(NZNumber).Extension = "3456")            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                phoneUtil.IsNumberMatch(nzNumber, "+643 331 6005 ext 3456"));
            // Check empty extensions are ignored.
            nzNumberAssert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
;
                phoneUtil.IsNumberMatch(nzNumber, "+6403 331 6005"));
            // Check variant with two proto buffers.
            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH, phoneUtil.IsNumberMatch(nzNumber, NZNumber));

            // Check raw_input, country_code_source and preferred_domestic_carrier_code are ignored.
            var brNumberOne = new PhoneNumber { CountryCode = 55, NationalNumber = (3121286979L }
                .SetCountryCodeSource(PhoneNumber.Types.CountryCodeSource.FromNumberWithPlusSign)
                .PreferredDomesticCarrierCode = "12"), RawInput = "012 3121286979"            var brNumberTwo = new PhoneNumber { CountryCode = 55, NationalNumber = (3121286979L }
                .SetCountryCodeSource(PhoneNumber.Types.CountryCodeSource.FromDefaultCountry)
                .PreferredDomesticCarrierCode = "14"), RawInput = "143121286979"            Assert.Equal(PhoneNumberUtil.MatchType.EXACT_MATCH,
                phoneUtil.IsNumberMatch(brNumberOne, brNumberTwo));
        }

        [Fact]
        public void TestIsNumberMatchNonMatches()
        {
            // Non-matches.
            Assert.Equal(PhoneNumberUtil.MatchType.NO_MATCH,
                phoneUtil.IsNumberMatch("03 331 6005", "03 331 6006"));
            Assert.Equal(PhoneNumberUtil.MatchType.NO_MATCH,
                 phoneUtil.IsNumberMatch("+800 1234 5678", "+1 800 1234 5678"));
            // Different country calling code, partial number match.
            Assert.Equal(PhoneNumberUtil.MatchType.NO_MATCH,
                phoneUtil.IsNumberMatch("+64 3 331-6005", "+16433316005"));
            // Different country calling code, same number.
            Assert.Equal(PhoneNumberUtil.MatchType.NO_MATCH,
                phoneUtil.IsNumberMatch("+64 3 331-6005", "+6133316005"));
            // Extension different, all else the same.
            Assert.Equal(PhoneNumberUtil.MatchType.NO_MATCH,
                phoneUtil.IsNumberMatch("+64 3 331-6005 extn 1234", "0116433316005#1235"));
            Assert.Equal(PhoneNumberUtil.MatchType.NO_MATCH,
                 phoneUtil.IsNumberMatch(
                     "+64 3 331-6005 extn 1234", "tel:+64-3-331-6005;ext=1235"));
            // NSN matches, but extension is different - not the same number.
            Assert.Equal(PhoneNumberUtil.MatchType.NO_MATCH,
                phoneUtil.IsNumberMatch("+64 3 331-6005 ext.1235", "3 331 6005#1234"));

            // Invalid numbers that can't be parsed.
            Assert.Equal(PhoneNumberUtil.MatchType.NOT_A_NUMBER,
                phoneUtil.IsNumberMatch("4", "3 331 6043"));
            Assert.Equal(PhoneNumberUtil.MatchType.NOT_A_NUMBER,
                phoneUtil.IsNumberMatch("+43", "+64 3 331 6005"));
            Assert.Equal(PhoneNumberUtil.MatchType.NOT_A_NUMBER,
                phoneUtil.IsNumberMatch("+43", "64 3 331 6005"));
            Assert.Equal(PhoneNumberUtil.MatchType.NOT_A_NUMBER,
                phoneUtil.IsNumberMatch("Dog", "64 3 331 6005"));
        }

        [Fact]
        public void TestIsNumberMatchNsnMatches()
        {
            // NSN matches.
            Assert.Equal(PhoneNumberUtil.MatchType.NSN_MATCH,
                phoneUtil.IsNumberMatch("+64 3 331-6005", "03 331 6005"));
            Assert.Equal(PhoneNumberUtil.MatchType.NSN_MATCH,
                 phoneUtil.IsNumberMatch(
                     "+64 3 331-6005", "tel:03-331-6005;isub=1234;phone-context=abc.nz"));
            Assert.Equal(PhoneNumberUtil.MatchType.NSN_MATCH,
                phoneUtil.IsNumberMatch(NZNumber, "03 331 6005"));
            // Here the second number possibly starts with the country calling code for New Zealand,
            // although we are unsure.
            var unchangedNzNumber = new PhoneNumber().MergeFrom(NZNumber).Build();
            Assert.Equal(PhoneNumberUtil.MatchType.NSN_MATCH,
                phoneUtil.IsNumberMatch(unchangedNzNumber, "(64-3) 331 6005"));
            // Check the phone number proto was not edited during the method call.
            Assert.Equal(NZNumber, unchangedNzNumber);

            // Here, the 1 might be a national prefix, if we compare it to the US number, so the resultant
            // match is an NSN match.
            Assert.Equal(PhoneNumberUtil.MatchType.NSN_MATCH,
                phoneUtil.IsNumberMatch(USNumber, "1-650-253-0000"));
            Assert.Equal(PhoneNumberUtil.MatchType.NSN_MATCH,
                phoneUtil.IsNumberMatch(USNumber, "6502530000"));
            Assert.Equal(PhoneNumberUtil.MatchType.NSN_MATCH,
                phoneUtil.IsNumberMatch("+1 650-253 0000", "1 650 253 0000"));
            Assert.Equal(PhoneNumberUtil.MatchType.NSN_MATCH,
                phoneUtil.IsNumberMatch("1 650-253 0000", "1 650 253 0000"));
            Assert.Equal(PhoneNumberUtil.MatchType.NSN_MATCH,
                phoneUtil.IsNumberMatch("1 650-253 0000", "+1 650 253 0000"));
            // For this case, the match will be a short NSN match, because we cannot assume that the 1 might
            // be a national prefix, so don't remove it when parsing.
            var randomNumber = new PhoneNumber { CountryCode = 41, NationalNumber = 6502530000L };
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                phoneUtil.IsNumberMatch(randomNumber, "1-650-253-0000"));
        }


        [Fact]
        public void TestIsNumberMatchShortNsnMatches()
        {
            // Short NSN matches with the country not specified for either one or both numbers.
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                phoneUtil.IsNumberMatch("+64 3 331-6005", "331 6005"));
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                 phoneUtil.IsNumberMatch("+64 3 331-6005", "tel:331-6005;phone-context=abc.nz"));
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                 phoneUtil.IsNumberMatch("+64 3 331-6005",
                     "tel:331-6005;isub=1234;phone-context=abc.nz"));
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                 phoneUtil.IsNumberMatch("+64 3 331-6005",
                     "tel:331-6005;isub=1234;phone-context=abc.nz;a=%A1"));
            // We did not know that the "0" was a national prefix since neither number has a country code,
            // so this is considered a SHORT_NSN_MATCH.
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                phoneUtil.IsNumberMatch("3 331-6005", "03 331 6005"));
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                phoneUtil.IsNumberMatch("3 331-6005", "331 6005"));
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                 phoneUtil.IsNumberMatch("3 331-6005", "tel:331-6005;phone-context=abc.nz"));
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                phoneUtil.IsNumberMatch("3 331-6005", "+64 331 6005"));
            // Short NSN match with the country specified.
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                phoneUtil.IsNumberMatch("03 331-6005", "331 6005"));
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                phoneUtil.IsNumberMatch("1 234 345 6789", "345 6789"));
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                phoneUtil.IsNumberMatch("+1 (234) 345 6789", "345 6789"));
            // NSN matches, country calling code omitted for one number, extension missing for one.
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                phoneUtil.IsNumberMatch("+64 3 331-6005", "3 331 6005#1234"));
            // One has Italian leading zero, one does not.
            var italianNumberOne = new PhoneNumber { CountryCode = 39, NationalNumber = 1234L, ItalianLeadingZero = (true }            var italianNumberTwo = new PhoneNumber { CountryCode = 39, NationalNumber = 1234L };
            Assert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
                phoneUtil.IsNumberMatch(italianNumberOne, italianNumberTwo));
            // One has an extension, the other has an extension of "".
            italianNumberOneAssert.Equal(PhoneNumberUtil.MatchType.SHORT_NSN_MATCH,
;
                phoneUtil.IsNumberMatch(italianNumberOne, italianNumberTwo));
        }

        [Fact]
        public void TestCanBeInternationallyDialled()
        {
            // We have no-international-dialling rules for the US in our test metadata that say that
            // toll-free numbers cannot be dialled internationally.
            Assert.False(phoneUtil.CanBeInternationallyDialled(USTollfree));

            // Normal US numbers can be internationally dialled.
            Assert.True(phoneUtil.CanBeInternationallyDialled(USNumber));

            // Invalid number.
            Assert.True(phoneUtil.CanBeInternationallyDialled(USLocalNumber));

            // We have no data for NZ - should return true.
            Assert.True(phoneUtil.CanBeInternationallyDialled(NZNumber));
            Assert.True(phoneUtil.CanBeInternationallyDialled(InternationalTollFree));
        }

        [Fact]
        public void TestIsAlphaNumber()
        {
            Assert.True(phoneUtil.IsAlphaNumber("1800 six-flags"));
            Assert.True(phoneUtil.IsAlphaNumber("1800 six-flags ext. 1234"));
            Assert.True(phoneUtil.IsAlphaNumber("+800 six-flags"));
            Assert.True(phoneUtil.IsAlphaNumber("180 six-flags"));
            Assert.False(phoneUtil.IsAlphaNumber("1800 123-1234"));
            Assert.False(phoneUtil.IsAlphaNumber("1 six-flags"));
            Assert.False(phoneUtil.IsAlphaNumber("18 six-flags"));
            Assert.False(phoneUtil.IsAlphaNumber("1800 123-1234 extension: 1234"));
            Assert.False(phoneUtil.IsAlphaNumber("+800 1234-1234"));
        }
    }
}
