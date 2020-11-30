namespace pb
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class t_BookDetails
    {
        string[] _ChapterDiscriptionArray;
        public string[] ChapterDiscriptionArray
        {
            get
            {
                if (_ChapterDiscriptionArray == null)
                {
                    ChapterDiscription = ChapterDiscription.Replace("\\","");
                    _ChapterDiscriptionArray = JsonHelper.JsonToObject<string[]>(ChapterDiscription);
                }
                return _ChapterDiscriptionArray;
            }
        }


        int[] _BannerIconArray;
        public int[] BannerIconArray
        {
            get
            {
                if (_BannerIconArray == null)
                {
                    _BannerIconArray = JsonHelper.JsonToObject<int[]>(this.BannerIcon);
                }
                return _BannerIconArray;
            }
        }

        public string[] GeneroButtonName;


        int[] _BookSearchTypeArray;
        public int[] BookSearchTypeArray
        {
            get
            {
                if (_BookSearchTypeArray == null)
                {
                    _BookSearchTypeArray = JsonHelper.JsonToObject<int[]>(this.BookSearchType);
                }
                return _BookSearchTypeArray;
            }
        }


        int[] _Type1Array;
        public int[] Type1Array
        {
            get
            {
                if (GeneroButtonName == null)
                {
                    AddButttonName();
                }

                if (_Type1Array == null)
                {
                    _Type1Array = JsonHelper.JsonToObject<int[]>(this.Type1);
                }
                return _Type1Array;
            }
        }



        private void AddButttonName()
        {
            GeneroButtonName = new string[12] {"Romance","LGBT","Action","Youth","Adventure","Drama","Comedy","Horror","18+","Fantasy","Suspense","Others"};
        }



        int[] _ChapterDivisionArray;
        public int[] ChapterDivisionArray
        {
            get
            {
                if (_ChapterDivisionArray == null)
                {
                    _ChapterDivisionArray = JsonHelper.JsonToObject<int[]>(this.ChapterDivision);
                }
                return _ChapterDivisionArray;
            }
        }

        string[] _BookCharacterArray;
        public string[] BookCharacterArray
        {
            get
            {
                if (_BookCharacterArray == null)
                {
                    _BookCharacterArray = JsonHelper.JsonToObject<string[]>(this.BookCharacterName);
                }
                return _BookCharacterArray;
            }
        }

        string[] _ChapterPriceArray;
        public string[] CharacterPricesArray
        {
            get
            {
                if (_ChapterPriceArray == null)
                {
                    _ChapterPriceArray = JsonHelper.JsonToObject<string[]>(this.ChapterPrice);
                }
                return _ChapterPriceArray;
            }
        }



    }
}