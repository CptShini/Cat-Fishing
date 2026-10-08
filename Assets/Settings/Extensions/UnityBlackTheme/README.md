# AutoSaver

The `AutoSaver.cs` script automatically saves the current scene at regular intervals or on play.

The file must be in the `Editor` folder to work.

The interval is set to 60 seconds (60 or 300 is recommended). Low intervals like 5 seconds will cause heavy editor lagging.

The file must be in the `Editor` folder to work.

























# UnityBlackTheme

<!--------------------------------------------------------------------------------------------------------------------------------------->
<!-- 🔖 Customization -->
<!--------------------------------------------------------------------------------------------------------------------------------------->

This is a custom dark theme for the Unity Editor. It provides a sleek and modern look while maintaining readability and usability, [link](https://github.com/haywirephoenix/UnityBlackTheme).

![image](https://github.com/haywirephoenix/UnityBlackTheme/assets/26271795/2e296832-a946-4b53-a7d4-a2cdc27aa661)

Drop the `Editor` folder in your project's Assets folder and Unity Editor will instantly turn black.

<!--------------------------------------------------------------------------------------------------------------------------------------->
<!-- 🔖 Customization -->
<!--------------------------------------------------------------------------------------------------------------------------------------->

The theme is fully customizable. You can modify the colors and styles by editing the `dark.uss` file located at:

    ./Editor/StyleSheets/Extensions/dark.uss

Below is a snippet of the `dark.uss` file that I have customized:

```css
/* ------------------------------------------------------------------- */
/* 🔖 Editor */
/* ------------------------------------------------------------------- */

.RenderTexture
{
    background-color: #0E1011 !important;
}

/* ------------------------------------------------------------------- */
/* 🔖 Tabs */
/* ------------------------------------------------------------------- */

/* Tabs container */
.dockHeader {
    background-color: #000 !important;
}

/* Tabs */
.dragtab-label {
    background-color: #12B08D66 !important;
}

/* ------------------------------------------------------------------- */
/* 🔖 Scene Object */
/* ------------------------------------------------------------------- */

/* Scene Active */
.SceneTopBarBg {
    background-color: #9e964da8 !important;
}

/* ------------------------------------------------------------------- */
/* 🔖 Components */
/* ------------------------------------------------------------------- */

/* Component h1 */
.IN-BigTitle       { background-color: #e3b2ed70 !important; }
.IN-BigTitle:hover { background-color: #e3b2ed90 !important; }

/* Component h2 */
.IN-Title       { background-color: #4466aa80 !important; }
.IN-Title:hover { background-color: #4466aaa0 !important;}

/* Component h3 */
.FoldoutHeader       { background-color: #86ff8250 !important; }
.FoldoutHeader:hover { background-color: #86ff8280 !important; }

/* ------------------------------------------------------------------- */
/* 🔖 X */
/* ------------------------------------------------------------------- */
```
