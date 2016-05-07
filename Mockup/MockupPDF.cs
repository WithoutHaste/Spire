using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

public static class PDFDemo
{
	public static void InitializeDemo(Panel parent)
	{
		int margin = 10;
		int leftSide = margin;
		int leftWidth = parent.Width/2 - margin;
		int rightSide = leftSide + leftWidth + margin;
		int rightWidth = leftWidth;
				
		Label title = MockupWindow.BuildLabel(ContentAlignment.TopCenter);
		title.Left = margin;
		title.Top = margin;
		title.Height = 25;
		title.Width = parent.Width - 4*margin;
		title.Font = new Font(new FontFamily("Times New Roman"), 16);
		title.Text = "Axiomata sive Leges Motus";
		title.Parent = parent;

		Label header1 = MockupWindow.BuildLabel(ContentAlignment.TopCenter);
		header1.Left = leftSide;
		header1.Top = title.Top + title.Height + 25;
		header1.Height = 20;
		header1.Width = leftWidth;
		header1.Font = new Font(new FontFamily("Times New Roman"), 14);
		header1.Text = "LEX 1.";
		header1.Parent = parent;

		Label p1A = MockupWindow.BuildLabel(ContentAlignment.TopLeft);
		p1A.Left = leftSide;
		p1A.Top = header1.Top + header1.Height + 5;
		p1A.Height = 60;
		p1A.Width = leftWidth;
		p1A.Font = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Italic);
		p1A.Text = "Corpus omne perseverare in statu suo quiescendi vel movendi uniformiter in directum, nisi quatenus illud a viribus impressis cogitur statum suum mutare.";
		p1A.Parent = parent;

		Label p1B = MockupWindow.BuildLabel(ContentAlignment.TopLeft);
		p1B.Left = leftSide;
		p1B.Top = p1A.Top + p1A.Height + 5;
		p1B.Height = 60;
		p1B.Width = leftWidth;
		p1B.Font = new Font(new FontFamily("Times New Roman"), 12);
		p1B.Text = "    Corpus omne perseverare in statu suo quiescendi vel movendi uniformiter in directum, nisi quatenus illud a viribus impressis cogitur statum suum mutare.";
		p1B.Parent = parent;

		Label header2 = MockupWindow.BuildLabel(ContentAlignment.TopCenter);
		header2.Left = leftSide;
		header2.Top = p1B.Top + p1B.Height + 10;
		header2.Height = 20;
		header2.Width = leftWidth;
		header2.Font = new Font(new FontFamily("Times New Roman"), 14);
		header2.Text = "LEX II.";
		header2.Parent = parent;

		Label p2A = MockupWindow.BuildLabel(ContentAlignment.TopLeft);
		p2A.Left = leftSide;
		p2A.Top = header2.Top + header2.Height + 5;
		p2A.Height = 60;
		p2A.Width = leftWidth;
		p2A.Font = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Italic);
		p2A.Text = "Mutationem motus proportionalem esse vi motrici impressa, && fieri secundum lineam rectam qua vis illa imprimitur.";
		p2A.Parent = parent;

		Label p2B = MockupWindow.BuildLabel(ContentAlignment.TopLeft);
		p2B.Left = leftSide;
		p2B.Top = p2A.Top + p2A.Height + 5;
		p2B.Height = 150;
		p2B.Width = leftWidth;
		p2B.Font = new Font(new FontFamily("Times New Roman"), 12);
		p2B.Text = "    Si vis aliqua motum quemvis generet; dupla duplum, tripla triplum generabit, sive simul && semel, sive gradatim && successive impressa fuerit. Et hic motus (quoniam in eandem semper plagam cum vi generatrice determinatur) si corpus antea movebatur, motui ejus vel conspiranti additur, vel contrario subducitur, vel obliquo oblique adjicitur, && cum eo secundum utriusque determinationem componitur.";
		p2B.Parent = parent;

		Label header3 = MockupWindow.BuildLabel(ContentAlignment.TopCenter);
		header3.Left = leftSide;
		header3.Top = p2B.Top + p2B.Height + 10;
		header3.Height = 20;
		header3.Width = leftWidth;
		header3.Font = new Font(new FontFamily("Times New Roman"), 14);
		header3.Text = "LEX III.";
		header3.Parent = parent;

		Label p3A = MockupWindow.BuildLabel(ContentAlignment.TopLeft);
		p3A.Left = leftSide;
		p3A.Top = header3.Top + header3.Height + 5;
		p3A.Height = 60;
		p3A.Width = leftWidth;
		p3A.Font = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Italic);
		p3A.Text = "Actioni contrariam semper && aqualem esse reactionem: sive corporum duorum actiones in se mutuo semper esse aquales && in partes contrarias dirigi.";
		p3A.Parent = parent;

		Label p3B = MockupWindow.BuildLabel(ContentAlignment.TopLeft);
		p3B.Left = leftSide;
		p3B.Top = p3A.Top + p3A.Height + 5;
		p3B.Height = 200;
		p3B.Width = leftWidth;
		p3B.Font = new Font(new FontFamily("Times New Roman"), 12);
		p3B.Text = "    Quicquid premit vel trahit alterum, tantundem ab eo premitur vel trahitur. Si quis lapidem digito premit, premitur && hujus digitus a lapide. Si equus lapidem funi alligatum trahit, retrahetur etiam && equus (ut ita dicam) aequaliter in lapidem: nam funis utrinque distentus eodem relaxandi se conatu urgebit equum versus lapidem, ac lapidem versus equum; tantumque impediet progressum unius quantum promovet pregressum alterius. Si corpus aliquod in corpus aliud impingens, motum ejus vi sua quomodocumque mutaverit, idem quoque vicissim in motu ejus vi sua quomodocunque mutaverit, idem quoque vicissim in motu prorio eandem mutationem in partem contratium vi alterius (ob aequalitatem pressionis mutuae) subibit. His actionibus aeqales fiunt mutationes, non velocitatem, sed motuum; scilicet in corporibus non aliunde impeditis. Mutationes enim velocitatum, in contratias itidem partes factae, quia motus aequaliter mutantur, sunt corporibus reciproce proportionales. Obtinet etiam haec lex in attractionibus, ut in scholio proximo probabitur.";
		p3B.Parent = parent;

		Label header4 = MockupWindow.BuildLabel(ContentAlignment.TopCenter);
		header4.Left = rightSide;
		header4.Top = title.Top + title.Height + 25;
		header4.Height = 20;
		header4.Width = rightWidth;
		header4.Font = new Font(new FontFamily("Times New Roman"), 14);
		header4.Text = "Corollarium I.";
		header4.Parent = parent;

		Label p4A = MockupWindow.BuildLabel(ContentAlignment.TopLeft);
		p4A.Left = rightSide;
		p4A.Top = header4.Top + header4.Height + 5;
		p4A.Height = 45;
		p4A.Width = rightWidth;
		p4A.Font = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Italic);
		p4A.Text = "Corpus viribus conjunctis diagonalem parallelogrammi eodem tempore describere, quo latera separatis.";
		p4A.Parent = parent;

		Button diagramButton = new Button();
		diagramButton.Top = p4A.Top + p4A.Height + 5;
		diagramButton.Height = 110;
		diagramButton.Width = 180;
		diagramButton.Left = rightSide + (rightWidth - diagramButton.Width);
		diagramButton.Image = Image.FromFile("PrincipiaMathematicaCorollariumI.png");
		diagramButton.ImageAlign = ContentAlignment.MiddleCenter;
		diagramButton.FlatStyle = FlatStyle.Flat;
		diagramButton.FlatAppearance.BorderSize = 0;
		diagramButton.Parent = parent;
		diagramButton.BringToFront();

		Label p4B = MockupWindow.BuildLabel(ContentAlignment.TopLeft);
		p4B.Left = rightSide;
		p4B.Top = p4A.Top + p4A.Height + 5;
		p4B.Height = 110;
		p4B.Width = rightWidth - diagramButton.Width - 5;
		p4B.Font = new Font(new FontFamily("Times New Roman"), 12);
		p4B.Text = "    Si corpus dato tempore, vi sola M in loco A impressa, ferretur uniformi cum motu ab A ad B, && vi sola N in eodem loco impresa, ferretur ab A ad C: compleatur";
		p4B.Parent = parent;
		
		Label p4C = MockupWindow.BuildLabel(ContentAlignment.TopLeft);
		p4C.Left = rightSide;
		p4C.Width = rightWidth;
		p4C.Top = p4B.Top + p4B.Height;
		p4C.Height = 230;
		p4C.Font = new Font(new FontFamily("Times New Roman"), 12);
		p4C.Text = "parallelogrammum ABDC, && vi utraque feretur corpus illud eodem tempore in diagonali ab A ad S. Nam quoniam vis N agit secundum lineam AC ipsi BD parallelam, haec vis per legem II nihil mutabit velocitatem accedendi ad lineam illam BD a vi altera genitam. Accedet igitur corpus eodem tempore ad lineam BD, sive vis N imprimatur, sive non; atque ideo in fine illius temporis reperietur alicubi in linea illa BD. Eodem argumento in fine temporis ejusdem reperietur alicubi in linea CD, && idcrico in utriusque lineae concursu D reperiri necesse est. Perget autem motu rectilineo ab A ad D per legem I.";
		p4C.Parent = parent;

		Label header5 = MockupWindow.BuildLabel(ContentAlignment.TopCenter);
		header5.Left = rightSide;
		header5.Top = p4C.Top + p4C.Height + 25;
		header5.Height = 20;
		header5.Width = rightWidth;
		header5.Font = new Font(new FontFamily("Times New Roman"), 14);
		header5.Text = "Corollarium II.";
		header5.Parent = parent;

		Label p5A = MockupWindow.BuildLabel(ContentAlignment.TopLeft);
		p5A.Left = rightSide;
		p5A.Top = header5.Top + header5.Height + 5;
		p5A.Height = 90;
		p5A.Width = rightWidth;
		p5A.Font = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Italic);
		p5A.Text = "Et hinc patet compositio vis directae AD ex viribus quisbusvis obliquis AB && BD, && vicissim resolutio vis cujusvis directae AD in obliquas quascunque AB && BD. Quae quidem compositio && resolutio abunde confirmatur ex mechanica.";
		p5A.Parent = parent;

	}	
}
