using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BookLoadingTexte : MonoBehaviour
{

    //private int shu = 0;

    private ArrayList myArraylist;
    private int shu = 0, LastNumber = -1;
    private Text MyText;
    private string sts;
    private bool isfirst = false;
    void OnEnable()
    {
        isfirst = true;
        MyText = transform.GetComponent<Text>();

        MyText.text = "";
        AddMyText();
        InvokeRepeating("InPutTexte", 0, 4);
    }

    void OnDisable()
    {
        //Debug.Log("清理");
        CancelInvoke("InPutTexte");
    }
    private void InPutTexte()
    {
        shu = Random.Range(0, myArraylist.Count);

        if (shu == LastNumber)
        {
            InPutTexte();
        }
        else
        {
            LastNumber = shu;
            sts = (string)myArraylist[shu];

            if (isfirst)
            {
                isfirst = false;
                MyText.color = new Color(1, 1, 1, 1);
            }
            else
            {
                MyText.color = new Color(1, 1, 1, 1);
            }

            MyText.DOColor(new Color(1, 1, 1, 0), 1).OnComplete(TexteToTrue);
            //Debug.Log("输出的文字是" + sts);
        }
    }

    private void TexteToTrue()
    {
        MyText.color = new Color(1, 1, 1, 0);
        MyText.DOColor(new Color(1, 1, 1, 1), 1f);
        MyText.text = sts;

    }


    private void AddMyText()
    {
        myArraylist = new ArrayList();
        myArraylist.Add("Discover new chapters updated every week!");
        myArraylist.Add("Diamond choices will unlock exclusive content from the game.");
        myArraylist.Add("Some choices will reward you with secret Diamonds.");
        myArraylist.Add("Bind your account with Facebook or Google to safeguard your progress!");
        myArraylist.Add("Need Diamonds? Watch a video and get 2 free diamonds each time!");
        myArraylist.Add("Do you have any suggestion? We want to know!");
        myArraylist.Add("Follow us on Facebook to stay tuned of our latest news!");
        myArraylist.Add("Need Diamonds? Go to the Store and get Free Diamonds after watching an AD!");
        myArraylist.Add("Get one free Diamond after finishing one chapter!");
        myArraylist.Add("Get free keys every day just by logging in!");
        myArraylist.Add("Log in everyday to collect free keys");
        myArraylist.Add("Discover new books and stories every week!");
        myArraylist.Add("Did you know that some choices include hidden diamonds? ");
        myArraylist.Add("Do you want us to publish your story? Get in touch with us!");
        myArraylist.Add("Your Diamond choices will be safe forever! You won't have to pay twice if you reset the book or the chapter.");
        myArraylist.Add("You'll get a free key every two hours to read new chapters!");
        myArraylist.Add("Tired of watching Ads? Make a purchase and they'll disappear forever!");
        myArraylist.Add("Be adventurous and try different books, you might like them!");
        myArraylist.Add("Buy all the clothes you like; our wardrobe is infinite!");
        myArraylist.Add("Don't know what to read? Play one of the recommended stories!");
        myArraylist.Add("We're finding the best outfits, please wait…");
        myArraylist.Add("Be patient! We're setting all the backgrounds in the story… ");
        myArraylist.Add("Share the game in Facebook to get free Diamonds!");
        myArraylist.Add("You can discuss with other players in our comments section!");
        myArraylist.Add("Express yourself with the emojis in the game!");
        myArraylist.Add("A story a day keeps the doctor away!");
        myArraylist.Add("No trees were harmed while making this book.");
        myArraylist.Add("A long time ago in a galaxy far, far away...oops, wrong story!");
        myArraylist.Add("Warning: Do not set yourself on fire.");
        myArraylist.Add("Please wait, we're making you a cookie.");
        myArraylist.Add("Generating next funny line...");
        myArraylist.Add("Entertaining you while you wait...");
        myArraylist.Add("Waiting for approval from The Creators...");
        myArraylist.Add("Creating Universe (this may take some time)");
        myArraylist.Add("Resistance is futile. Prepare to be amazed!");
        myArraylist.Add("Calculating meaning of life...");
        myArraylist.Add("And in her smile I see something more beautiful than the stars.");
        myArraylist.Add("When someone loves you, the way they talk about you is different. You feel safe and comfortable.");
        myArraylist.Add("Some women choose to follow men, and some women choose to follow their dreams.");
        myArraylist.Add("The very essence of romance is uncertainty.");
        myArraylist.Add("If you love somebody, let them go, for if they return, they were always yours.");
        myArraylist.Add("When love is not madness it is not love.");
        myArraylist.Add("A flower cannot blossom without sunshine, and woman cannot live without love.");
        myArraylist.Add("I love you without knowing how, or when, or from where. I love you simply, without problems or pride.");
        myArraylist.Add("Every great love starts with a great story...");
        myArraylist.Add("Come sleep with me: We won't make Love, Love will make us.");
        myArraylist.Add("If you want to keep a secret, you must also hide it from yourself.");
        myArraylist.Add("If someone wants you, nothing can keep you away. If he doesn't want you, nothing can make him stay.");
        myArraylist.Add("Attitude is a choice. Happiness is a choice. Whatever choice you make makes you. Choose wisely.");
        myArraylist.Add("You cannot control the behavior of others, but you can choose how you respond to it.");
        myArraylist.Add("Choose your love. Love your choice.");
        myArraylist.Add("You can do anything you want. Be with anyone you want. And it'll be your choice.");
        myArraylist.Add("May your choices reflect your hopes, not your fears.");
        myArraylist.Add("The right choice is hardly ever the easy choice.");
        myArraylist.Add("If you have to convince someone to stay with you then they have already left.");
        myArraylist.Add("A friend is someone who knows all about you and still loves you.");
        myArraylist.Add("It is better to be hated for what you are than to be loved for what you are not.");
        myArraylist.Add("Being deeply loved by someone gives you strength, while loving someone deeply gives you courage.");
        myArraylist.Add("True love happens accidentally, in a heartbeat, in a single flashing, throbbing moment.");
        myArraylist.Add("You've got to live like there's nobody watching.");
        myArraylist.Add("Love like you'll never be hurt.");
        myArraylist.Add("You only live once, but if you do it right, once is enough.");
        myArraylist.Add("It is not a lack of love, but a lack of friendship that makes unhappy marriages.");
        myArraylist.Add("Surprise! The man of her dreams is a girl.");
        myArraylist.Add("The only queer people are those who don't love anybody.");
        myArraylist.Add("Why do we always fall in love with things we can't have?");
        myArraylist.Add("Love yourself, whatever makes you different, and use it to make you stand out.");
        myArraylist.Add("Love is a wild fire that cannot be contained by any mere element known.");
        myArraylist.Add("Bisexuality immediately doubles your chances for a date on Saturday night.");
        myArraylist.Add("Nobody gives you power. You just take it.");
        myArraylist.Add("Choose your love interest carefully. From this choice will come 90 percent of all your happiness or misery.");
        myArraylist.Add("The story changes the moment you make a new, congruent, and committed decision.");
        myArraylist.Add("Sometimes it's the smallest decisions that can change your life forever.");
        myArraylist.Add("Life is a matter of choices, and every choice you make makes you.");

    }
}
