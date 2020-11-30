namespace AB
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    [Serializable]
    public class UVDetail
    {
        public string Name;

        public Vector2 uvTL;

        public Vector2 uvTR;

        public Vector2 uvBL;

        public Vector2 uvBR;

        public bool rotate;

        public int x;

        public int y;

        public int width;

        public int height;

        public static UVDetail Create(Sprite spt)
        {
            UVDetail ui = new UVDetail();
            ui.Name = spt.name;
            ui.width = (int)(spt.rect.size.x * 1000);
            ui.height = (int)(spt.rect.size.y * 1000);

            ui.x = (int)(spt.pivot.x * 1000);
            ui.y = (int)(spt.pivot.y * 1000);

            float minx = spt.uv[0].x;
            float maxx = spt.uv[0].x;
            float miny = spt.uv[0].y;
            float maxy = spt.uv[0].y;
            foreach (var v in spt.uv)
            {
                minx = Mathf.Min(minx, v.x);
                maxx = Mathf.Max(maxx, v.x);

                miny = Mathf.Min(miny, v.y);
                maxy = Mathf.Max(maxy, v.y);
            }
            ui.uvBL = new Vector2(minx, miny);//0,0
            ui.uvBR = new Vector2(maxx, miny);//1,0

            ui.uvTL = new Vector2(minx, maxy);//0,1
            ui.uvTR = new Vector2(maxx, maxy);//1,1
            return ui;
        }
    }
}