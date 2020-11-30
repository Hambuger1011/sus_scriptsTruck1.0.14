// By Kostiantyn Dvornik
// kostiantyn-dvornik.blogspot.com
// kostiantyn.dvornik@gmail.com
// Google me too
// Dont steal this guys, it's not cool to steal code

using UnityEngine;
using System.Collections;

public class KDManipulator {
				
	//Manipulator data
	public int[] minX;
    public int[] minY;

    public int[] maxX;
    public int[] maxY;
	
	private Color32 minFilterColor;
	private Color32 maxFilterColor;
	
	public Color32[] thePixels;
	public Texture2D filtTexture;
	
    private int[] PixelCount;

    private int[,] LabelBitmap;

    private int MaxLabelID;
        
    public int MaxID;

   	private int MaxArea;
    public int BiggestAreaID;

    public int StreamWidth;
    public int StreamHeight;
	
	
	/////////////////////////////////////////
	public int GetArea(int ID){
        return PixelCount[ID];
    }

    public int GetMaxX(int ID){		
         return maxX[ID];
    }

    public int GetMaxY(int ID){
         return maxY[ID];
    }

    public int GetMinX(int ID){
       return minX[ID];
	}

	public int GetMinY(int ID){

            return minY[ID];

    }
	
	// Use this for initialization
	public KDManipulator ( int inStreamWidth, int inStreamHeight) {
	
		StreamWidth = inStreamWidth;
		StreamHeight = inStreamHeight;
        
		// Allocate an arrays         
        minX = new int[StreamWidth * StreamHeight / 4];
        minY = new int[StreamWidth * StreamHeight / 4];

        maxX = new int[StreamWidth * StreamHeight / 4];
        maxY = new int[StreamWidth * StreamHeight / 4];

        PixelCount = new int[StreamWidth * StreamHeight / 4];
		
        LabelBitmap = new int[StreamWidth, StreamHeight];
				 
        // Fill arrays
        for (int i = 1; i < StreamWidth * StreamHeight / 4; i++)
        {
           minX[i] = int.MaxValue;
           minY[i] = int.MaxValue;

           maxX[i] = int.MinValue;
           maxY[i] = int.MinValue;

         }
				
        System.Array.Clear(PixelCount, 0, StreamWidth * StreamHeight / 4);
        System.Array.Clear(LabelBitmap, 0, StreamWidth * StreamHeight);

        MaxLabelID = 0;
        MaxID = 0;

        BiggestAreaID = 0;
        MaxArea = 0;
		
		// Set filter colors
		minFilterColor = new Color32( 100, 0, 0, 255 );
        maxFilterColor = new Color32( 255, 70, 70, 255 ); 			
		
	}
	
	// Use this for initialization
	public KDManipulator ( int inStreamWidth, int inStreamHeight, Color32 inMinFilterColor, Color32 inMaxFilterColor) {
	
		StreamWidth = inStreamWidth;
		StreamHeight = inStreamHeight;
        
		// Allocate an arrays         
        minX = new int[StreamWidth * StreamHeight / 4];
        minY = new int[StreamWidth * StreamHeight / 4];

        maxX = new int[StreamWidth * StreamHeight / 4];
        maxY = new int[StreamWidth * StreamHeight / 4];

        PixelCount = new int[StreamWidth * StreamHeight / 4];
		
        LabelBitmap = new int[StreamWidth, StreamHeight];
				 
        // Fill arrays
        for (int i = 1; i < StreamWidth * StreamHeight / 4; i++)
        {
           minX[i] = int.MaxValue;
           minY[i] = int.MaxValue;

           maxX[i] = int.MinValue;
           maxY[i] = int.MinValue;

         }
				
        System.Array.Clear(PixelCount, 0, StreamWidth * StreamHeight / 4);
        System.Array.Clear(LabelBitmap, 0, StreamWidth * StreamHeight);

        MaxLabelID = 0;
        MaxID = 0;

        BiggestAreaID = 0;
        MaxArea = 0;
		
		// Set filter colors
		minFilterColor = inMinFilterColor;
        maxFilterColor = inMaxFilterColor;		
		
	}
	
	
	public void ClearData()
    {		
     	//Fill arrays
        for (int i = 1; i < StreamWidth * StreamHeight / 4; i++)
        {
          minX[i] = int.MaxValue;
          minY[i] = int.MaxValue;

          maxX[i] = int.MinValue;
          maxY[i] = int.MinValue;

         }						
		
         System.Array.Clear(PixelCount, 0, StreamWidth * StreamHeight / 4);
         System.Array.Clear(LabelBitmap, 0, StreamWidth * StreamHeight);

         MaxLabelID = 0;
         MaxID = 0;

         BiggestAreaID = 0;
         MaxArea = 0;
         
        }
	// Next code is about manipulator
	bool IsThereLabelNeighbours( int x, int y ){
            
		//Based on 4 neigbours
        if ( LabelBitmap[x, y - 1] + LabelBitmap[x + 1, y] + LabelBitmap[x, y + 1] + LabelBitmap[x - 1, y] > 0) return true;
        else return false;
		
		
     }

     void RecalculateCoordinates(int x, int y, int ID){
		
      // Default stuff
      if (x < minX[ID]) minX[ID] = x;
      if (y < minY[ID]) minY[ID] = y;

      if (x > maxX[ID]) maxX[ID] = x;
      if (y > maxY[ID]) maxY[ID] = y;

      }


      int FindLowestNeighboursLabel(int x, int y){
        
		int MinLabel;
        if (LabelBitmap[x, y] > 0) MinLabel = LabelBitmap[x, y];
        MinLabel = int.MaxValue;
		
		int leftLabel = LabelBitmap[x - 1, y];
		int bottomLabel = LabelBitmap[x, y - 1];
		int rightLabel = LabelBitmap[x + 1, y];
		int topLabel = LabelBitmap[x, y + 1];
		
        //if (LabelBitmap[x - 1, y] > 0 && LabelBitmap[x - 1, y] < MinLabel) MinLabel = LabelBitmap[x - 1, y];
		if ( leftLabel > 0 && leftLabel < MinLabel) MinLabel = leftLabel;
        if ( bottomLabel > 0 && bottomLabel < MinLabel) MinLabel = bottomLabel;
        if ( rightLabel > 0 && rightLabel < MinLabel) MinLabel = rightLabel;
        if ( topLabel > 0 && topLabel < MinLabel) MinLabel = topLabel;

        return MinLabel;	
		
       }
	
	// Apply filtering
	void FilterTexture( ){
					
		for ( int i=0; i< thePixels.Length; i++){																					
		
			if ( (thePixels[i].r < minFilterColor.r || thePixels[i].r > maxFilterColor.r) ||
				 (thePixels[i].g < minFilterColor.g || thePixels[i].g > maxFilterColor.g) ||
				 (thePixels[i].b < minFilterColor.b || thePixels[i].b > maxFilterColor.b) ) thePixels[i].a = 0;
		}		
		
	}
	
	// Find one object	
    public void ProcessImage( ref WebCamTexture inTexture ){
		
		// Get pixels
		thePixels = inTexture.GetPixels32();
		
		// Apply the filter
		FilterTexture();
																
        // First iteration on witch we create all labels
        int CurrentLabelID = 0;
        MaxLabelID = 0;
		
		
        // Now go throw all texture
        for (int yy = 2; yy < StreamHeight - 2; yy++)
        {
			for (int xx = 2; xx < StreamWidth - 2; xx++)
            {
				                    
					if ( thePixels[ yy * StreamWidth + xx].a != 0 )
                    {	
					
                        if ( IsThereLabelNeighbours( xx, yy ) )
                        {							
                            // Find neighbour labels
                            CurrentLabelID = FindLowestNeighboursLabel( xx, yy );

                            // Assign to curent pixel neighbours ID
                            LabelBitmap[xx, yy] = CurrentLabelID;                            

                        }
                        else
                        {						
                            // Create new ID
                            MaxLabelID++;
                            CurrentLabelID = MaxLabelID;

                            // Assign to neighbors new IDs
                            LabelBitmap[xx, yy] = CurrentLabelID;                         
                        }
						
                     }
                     
                    
              }
          }
         
					
            // Second iteration on witch all labels get union            
			for (int yy = 2; yy < inTexture.height - 2; yy++)
            {
                for (int xx = inTexture.width - 2; xx > 2; xx--)
                {
                    if ( thePixels[ yy * StreamWidth + xx].a != 0 )
                    {
                        if (IsThereLabelNeighbours(xx, yy))
                        {
                            // Find neighbour labels
                            CurrentLabelID = FindLowestNeighboursLabel( xx, yy );

                            // Assign to curent pixel neighbours ID's
                            LabelBitmap[xx, yy] = CurrentLabelID;

                            // Now calculate each label area
                            PixelCount[CurrentLabelID]++;

                            if (PixelCount[CurrentLabelID] > MaxArea)
                            {

                                BiggestAreaID = CurrentLabelID;
                                MaxArea = PixelCount[CurrentLabelID];

                            }

                            // Recalculate coordinates
                            RecalculateCoordinates(xx, yy, CurrentLabelID);

                        }
                    }
                }
            }			
		
           	
        }	
        
}
