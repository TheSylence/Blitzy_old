﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Blitzy.Model;
using Blitzy.Plugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class CommandManager_Tests : Plugins.PluginTestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void ExecutionCountTest()
		{
			using( Settings settings = new Settings( ConnectionFactory ) )
			using( PluginManager plugins = new PluginManager( this, ConnectionFactory ) )
			{
				Mocks.MockPlugin plug = new Mocks.MockPlugin();
				using( CommandManager mgr = new CommandManager( ConnectionFactory, settings, plugins ) )
				{
					CommandItem item = CommandItem.Create( "lorem", "", plug );

					mgr.UpdateExecutionCount( item );
					Assert.AreEqual( 1, mgr.GetCommandExecutionCount( item ) );
					mgr.UpdateExecutionCount( item );
					Assert.AreEqual( 2, mgr.GetCommandExecutionCount( item ) );

					mgr.ResetExecutionCount();
					Assert.AreEqual( 0, mgr.GetCommandExecutionCount( item ) );
				}
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void PerformanceTest()
		{
			using( Settings settings = new Settings( ConnectionFactory ) )
			using( PluginManager plugins = new PluginManager( this, ConnectionFactory ) )
			{
				Mocks.MockPlugin plug = new Mocks.MockPlugin();

				using( CommandManager mgr = new CommandManager( ConnectionFactory, settings, plugins ) )
				{
					string[] names = TestData.Split( ',' );
					int totalCount = names.Length;
					int testCount = totalCount / 20;
					List<string> tests = new List<string>( testCount );
					Random rand = new Random();

					foreach( string name in names )
					{
						if( rand.Next( totalCount ) < testCount )
						{
							tests.Add( name );
						}
						mgr.AvailableCommands.Add( CommandItem.Create( name, name, plug ) );
					}

					Stopwatch timer = new Stopwatch();
					foreach( string test in tests )
					{
						timer.Start();
						mgr.Clear( true );
						mgr.SearchItems( test );
						timer.Stop();
					}

					TimeSpan elapsed = timer.Elapsed;
					TimeSpan maxSpan = TimeSpan.FromMilliseconds( 500 );

					Assert.IsTrue( elapsed <= maxSpan, string.Format( "{0} of {1}: {2}", testCount, totalCount, elapsed.ToString() ) );
				}
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void SearchTest()
		{
			using( Settings settings = new Settings( ConnectionFactory ) )
			using( PluginManager plugins = new PluginManager( this, ConnectionFactory ) )
			{
				Mocks.MockPlugin plug = new Mocks.MockPlugin();

				using( CommandManager mgr = new CommandManager( ConnectionFactory, settings, plugins ) )
				{
					mgr.AvailableCommands.Add( CommandItem.Create( "lorem", "", plug ) );
					mgr.AvailableCommands.Add( CommandItem.Create( "ipsum", "", plug ) );
					mgr.AvailableCommands.Add( CommandItem.Create( "dolor", "", plug ) );
					mgr.AvailableCommands.Add( CommandItem.Create( "sit", "", plug ) );
					mgr.AvailableCommands.Add( CommandItem.Create( "amet", "", plug ) );
					mgr.AvailableCommands.Add( CommandItem.Create( "consetetur", "", plug ) );

					mgr.Clear( true );
					mgr.SearchItems( "dol" );
					Assert.AreEqual( 1, mgr.Items.Count );
					Assert.AreEqual( "dolor", mgr.Items.First().Name );

					mgr.Clear( true );
					mgr.SearchItems( "lorem" );
					Assert.AreEqual( 1, mgr.Items.Count );
					Assert.AreEqual( "lorem", mgr.Items.First().Name );

					//mgr.Clear( true );
					mgr.SearchItems( "lorem" + mgr.Separator + "lo" );
					Assert.AreEqual( 1, mgr.Items.Count );
					Assert.AreEqual( "lorem", mgr.Items.First().Name );
				}
			}
		}

		private const string TestData = "a,an,able,about,above,abuse,accept,accident,accuse,across,act,activist,actor,add,administration,admit,adult,advertise,advise,affect,afraid,after,again,against,age,agency,aggression,ago,agree,agriculture,aid,aim,air,air force,airplane,airport,album,alcohol,alive,all,ally,almost,alone,along,already,also,although,always,ambassador,amend,ammunition,among,amount,anarchy,ancestor,ancient,and,anger,animal,anniversary,announce,another,answer,any,apologize,appeal,appear,appoint,approve,archeology,area,argue,arms,army,around,arrest,arrive,art,artillery,as,ash,ask,assist,astronaut,astronomy,asylum,at,atmosphere,attach,attack,attempt,attend,attention,automobile,autumn,available,average,avoid,awake,award,away,baby,back,bad,balance,ball,balloon,ballot,ban,bank,bar,barrier,base,battle,be,beat,beauty,because,become,bed,before,begin,behavior,behind,believe,belong,below,best,betray,better,between,big,bill,biology,bird,bite,black,blame,bleed,blind,block,blood,blow,blue,boat,body,boil,bomb,bone,book,border,born,borrow,both,bottle,bottom,box,boy,boycott,brain,brave,bread,break,breathe,bridge,brief,bright,bring,broadcast,brother,brown,budget,build,building,bullet,burn,burst,bury,bus,business,busy,but,buy,by,cabinet,call,calm,camera,camp,campaign,can,cancel,cancer,candidate,capital,capture,car,care,career,careful,carry,case,cat,catch,cause,ceasefire,celebrate,center,century,ceremony,chairman,champion,chance,change,charge,chase,cheat,cheer,chemicals,chemistry,chief,child,children,choose,circle,citizen,city,civilian,civil rights,claim,clash,class,clean,clear,clergy,climate,climb,clock,close,cloth,clothes,cloud,coal,coalition,coast,coffee,cold,collapse,collect,college,colony,color,combine,come,command,comment,committee,common,communicate,community,company,compare,compete,complete,complex,compromise,computer,concern,condemn,condition,conference,confirm,conflict,congratulate,Congress,connect,conservative,consider,constitution,contact,contain,container,continent,continue,control,convention,cook,cool,cooperate,copy,corn,correct,corruption,cost,cotton,count,country,court,cover,cow,crash,create,creature,credit,crew,crime,criminal,crisis,criticize,crops,cross,crowd,crush,cry,culture,cure,curfew,current,custom,customs,cut,dam,damage,dance,danger,dark,date,daughter,day,dead,deaf,deal,debate,debt,decide,declare,decrease,deep,defeat,defend,deficit,define,degree,delay,delegate,demand,democracy,demonstrate,denounce,deny,depend,deplore,deploy,depression,describe,desert,design,desire,destroy,detail,detain,develop,device,dictator,die,diet,different,difficult,dig,dinner,diplomat,direct,direction,dirt,disappear,disarm,disaster,discover,discrimination,discuss,disease,dismiss,dispute,dissident,distance,dive,divide,do,doctor,document,dog,dollar,donate,door,double,down,dream,drink,drive,drop,drown,drug,dry,during,dust,duty,each,early,earn,earth,earthquake,ease,east,easy,eat,ecology,economy,edge,education,effect,effort,egg,either,elect,electricity,embassy,embryo,emergency,emotion,employ,empty,end,enemy,energy,enforce,engine,engineer,enjoy,enough,enter,environment,equal,equipment,escape,especially,establish,estimate,ethnic,evaporate,even,event,ever,every,evidence,evil,exact,examine,example,excellent,except,exchange,excuse,execute,exercise,exile,exist,expand,expect,expel,experience,experiment,expert,explain,explode,explore,export,express,extend,extra,extraordinary,extreme,extremist,face,fact,factory,fail,fair,fall,false,family,famous,fan,far,farm,fast,fat,father,favorite,fear,federal,feed,feel,female,fence,fertile,few,field,fierce,fight,fill,film,final,financial,find,fine,finish,fire,fireworks,firm,first,fish,fit,fix,flag,flat,flee,float,flood,floor,flow,flower,fluid,fly,fog,follow,food,fool,foot,for,force,foreign,forest,forget,forgive,form,former,forward,free,freedom,freeze,fresh,friend,frighten,from,front,fruit,fuel,full,fun,funeral,future,gain,game,gas,gather,general,generation,genocide,gentle,get,gift,girl,give,glass,go,goal,god,gold,good,goods,govern,government,grain,grass,gray,great,green,grind,ground,group,grow,guarantee,guard,guerrilla,guide,guilty,gun,hair,half,halt,hang,happen,happy,hard,harm,harvest,hat,hate,have,he,head,headquarters,heal,health,hear,heat,heavy,helicopter,help,here,hero,hide,high,hijack,hill,history,hit,hold,hole,holiday,holy,home,honest,honor,hope,horrible,horse,hospital,hostage,hostile,hot,hotel,hour,house,how,however,huge,human,humor,hunger,hunt,hurry,hurt,husband,I,ice,idea,identify,if,ignore,illegal,imagine,immediate,immigrant,import,important,improve,in,incident,incite,include,increase,independent,individual,industry,infect,inflation,influence,inform,information,inject,injure,innocent,insane,insect,inspect,instead,instrument,insult,intelligence,intelligent,intense,interest,interfere,international,Internet,intervene,invade,invent,invest,investigate,invite,involve,iron,island,issue,it,jail,jewel,job,join,joint,joke,judge,jump,jury,just,justice,keep,kick,kidnap,kill,kind,kiss,knife,know,knowledge,labor,laboratory,lack,lake,land,language,large,last,late,laugh,launch,law,lead,leak,learn,leave,left,legal,legislature,lend,less,let,letter,level,liberal,lie,life,lift,light,lightning,like,limit,line,link,liquid,list,listen,literature,little,live,load,loan,local,lonely,long,look,lose,loud,love,low,loyal,luck,machine,magazine,mail,main,major,majority,make,male,man,manufacture,many,map,march,mark,market,marry,mass,mate,material,mathematics,matter,may,mayor,meal,mean,measure,meat,media,medicine,meet,melt,member,memorial,memory,mental,message,metal,method,microscope,middle,militant,military,militia,milk,mind,mine,mineral,minister,minor,minority,minute,miss,missile,missing,mistake,mix,mob,model,moderate,modern,money,month,moon,moral,more,morning,most,mother,motion,mountain,mourn,move,movement,movie,much,murder,music,must,mystery,name,narrow,nation,native,natural,nature,navy,near,necessary,need,negotiate,neighbor,neither,neutral,never,new,news,next,nice,night,no,noise,nominate,noon,normal,north,not,note,nothing,now,nowhere,nuclear,number,obey,object,observe,occupy,ocean,of,off,offensive,offer,office,officer,official,often,oil,old,on,once,only,open,operate,opinion,oppose,opposite,oppress,or,orbit,order,organize,other,our,oust,out,over,overthrow,owe,own,pain,paint,paper,parachute,parade,pardon,parent,parliament,part,partner,party,pass,passenger,passport,past,path,patient,pay,peace,people,percent,perfect,perform,period,permanent,permit,person,persuade,physical,physics,picture,piece,pig,pilot,pipe,place,plan,planet,plant,plastic,play,please,plenty,plot,poem,point,poison,police,policy,politics,pollute,poor,popular,population,port,position,possess,possible,postpone,pour,poverty,power,praise,pray,predict,pregnant,present,president,press,pressure,prevent,price,prison,private,prize,probably,problem,process,produce,profession,professor,profit,program,progress,project,promise,propaganda,property,propose,protect,protest,prove,provide,public,publication,publish,pull,pump,punish,purchase,pure,purpose,push,put,quality,question,quick,quiet,race,radar,radiation,radio,raid,railroad,rain,raise,rape,rare,rate,reach,react,read,ready,real,realistic,reason,reasonable,rebel,receive,recent,recession,recognize,record,recover,red,reduce,reform,refugee,refuse,register,regret,reject,relations,release,religion,remain,remains,remember,remove,repair,repeat,report,represent,repress,request,require,rescue,research,resign,resist,resolution,resource,respect,responsible,rest,restaurant,restrain,restrict,result,retire,return,revolt,rice,rich,ride,right,riot,rise,risk,river,road,rob,rock,rocket,roll,room,root,rope,rough,round,rub,rubber,ruin,rule,run,rural,sabotage,sacrifice,sad,safe,sail,sailor,salt,same,sand,satellite,satisfy,save,say,school,science,sea,search,season,seat,second,secret,security,see,seed,seeking,seem,seize,self,sell,Senate,send,sense,sentence,separate,series,serious,serve,service,set,settle,several,severe,sex,shake,shape,share,sharp,she,sheep,shell,shelter,shine,ship,shock,shoe,shoot,short,should,shout,show,shrink,sick,sickness,side,sign,signal,silence,silver,similar,simple,since,sing,single,sink,sister,sit,situation,size,skeleton,skill,skin,sky,slave,sleep,slide,slow,small,smash,smell,smoke,smooth,snow,so,social,soft,soil,soldier,solid,solve,some,son,soon,sort,sound,south,space,speak,special,speech,speed,spend,spill,spirit,split,sport,spread,spring,spy,square,stab,stand,star,start,starve,state,station,statue,stay,steal,steam,steel,step,stick,still,stone,stop,store,storm,story,stove,straight,strange,street,stretch,strike,strong,structure,struggle,study,stupid,subject,submarine,substance,substitute,subversion,succeed,such,sudden,suffer,sugar,suggest,suicide,summer,sun,supervise,supply,support,suppose,suppress,sure,surface,surplus,surprise,surrender,surround,survive,suspect,suspend,swallow,swear in,sweet,swim,sympathy,system,take,talk,tall,tank,target,taste,tax,tea,teach,team,tear,technical,technology,telephone,telescope,television,tell,temperature,temporary,tense,term,terrible,territory,terror,terrorist,test,than,thank,that,the,theater,them,then,theory,there,these,they,thick,thin,thing,think,third,this,threaten,through,throw,tie,time,tired,to,today,together,tomorrow,tonight,too,tool,top,torture,total,touch,toward,town,trade,tradition,traffic,tragic,train,transport,transportation,trap,travel,treason,treasure,treat,treatment,treaty,tree,trial,tribe,trick,trip,troops,trouble,truce,truck,true,trust,try,tube,turn,under,understand,unite,universe,university,unless,until,up,urge,urgent,us,use,usual,vacation,vaccine,valley,value,vegetable,vehicle,version,very,veto,victim,victory,video,village,violate,violence,visa,visit,voice,volcano,volunteer,vote,wages,wait,walk,wall,want,war,warm,warn,wash,waste,watch,water,wave,way,we,weak,wealth,weapon,wear,weather,Web site,week,weigh,welcome,well,west,wet,what,wheat,wheel,when,where,whether,which,while,white,who,whole,why,wide,wife,wild,will,willing,win,wind,window,winter,wire,wise,wish,with,withdraw,without,witness,woman,wonder,wonderful,wood,word,work,world,worry,worse,worth,wound,wreck,wreckage,write,wrong,year,yellow,yes,yesterday,yet,you,young,zero,zoo,der,die,und,in,den,von,zu,das,mit,sich,des,auf,für,ist,im,dem,nicht,ein,Die,eine,als,auch,es,an,werden,aus,er,hat,daß,sie,nach,wird,bei,einer,Der,um,am,sind,noch,wie,einem,über,einen,Das,so,Sie,zum,war,haben,nur,oder,aber,vor,zur,bis,mehr,durch,man,sein,wurde,sei,In,Prozent,hatte,kann,gegen,vom,können,schon,wenn,habe,seine,Mark,ihre,dann,unter,wir,soll,ich,eines,Es,Jahr,zwei,Jahren,diese,dieser,wieder,keine,Uhr,seiner,worden,Und,will,zwischen,Im,immer,Millionen,Ein,was,sagte,Er,gibt,alle,DM,diesem,seit,muß,wurden,beim,doch,jetzt,waren,drei,Jahre,Mit,neue,neuen,damit,bereits,da,Auch,ihr,seinen,müssen,ab,ihrer,Nach,ohne,sondern,selbst,ersten,nun,etwa,Bei,heute,ihren,weil,ihm,seien,Menschen,Deutschland,anderen,werde,Ich,sagt,Wir,Eine,rund,Für,Aber,ihn,Ende,jedoch,Zeit,sollen,ins,Wenn,So,seinem,uns,Stadt,geht,Doch,sehr,hier,ganz,erst,wollen,Berlin,vor allem,sowie,hatten,kein,deutschen,machen,lassen,Als,Unternehmen,andere,ob,dieses,steht,dabei,wegen,weiter,denn,beiden,einmal,etwas,Wie,nichts,allerdings,vier,gut,viele,wo,viel,dort,alles,Auf,wäre,SPD,kommt,vergangenen,denen,fast,fünf,könnte,nicht nur,hätten,Frau,Am,dafür,kommen,diesen,letzten,zwar,Diese,großen,dazu,Von,Mann,Da,sollte,würde,also,bisher,Leben,Milliarden,Welt,Regierung,konnte,ihrem,Frauen,während,Land,zehn,würden,stehen,ja,USA,heißt,dies,zurück,Kinder,dessen,ihnen,deren,sogar,Frage,gewesen,erste,gab,liegt,gar,davon,gestern,geben,Teil,Polizei,dass,hätte,eigenen,kaum,sieht,große,Denn,weitere,Was,sehen,macht,Angaben,weniger,gerade,läßt,Geld,München,deutsche,allen,darauf,wohl,später,könne,deshalb,aller,kam,Arbeit,mich,gegenüber,nächsten,bleibt,wenig,lange,gemacht,Wer,Dies,Fall,mir,gehen,Berliner,mal,Weg,CDU,wollte,sechs,keinen,Woche,dagegen,alten,möglich,gilt,erklärte,müsse,Dabei,könnten,Geschichte,zusammen,finden,Tag,Art,erhalten,Man,Dollar,Wochen,jeder,nie,bleiben,besonders,Jahres,Deutschen,Den,Zu,zunächst,derzeit,allein,deutlich,Entwicklung,weiß,einige,sollten,Präsident,geworden,statt,Bonn,Platz,inzwischen,Nur,Freitag,Um,pro,seines,Damit,Montag,Europa,schließlich,Sonntag,einfach,gehört,eher,oft,Zahl,neben,hält,weit,Partei,meisten,Thema,zeigt,Politik,Aus,zweiten,Januar,insgesamt,je,mußte,Anfang,hinter,ebenfalls,ging,Mitarbeiter,darüber,vielen,Ziel,darf,Seite,fest,hin,erklärt,Namen,Haus,An,Frankfurt,Gesellschaft,Mittwoch,damals,Dienstag,Hilfe,Mai,Markt,Seit,Tage,Donnerstag,halten,gleich,nehmen,solche,Entscheidung,besser,alte,Leute,Ergebnis,Samstag,Daß,sagen,System,März,tun,Monaten,kleinen,lang,Nicht,knapp,bringen,wissen,Kosten,Erfolg,bekannt,findet,daran,künftig,wer,acht,Grünen,schnell,Grund,scheint,Zukunft,Stuttgart,bin,liegen,politischen,Gruppe,Rolle,stellt,Juni,sieben,September,nämlich,Männer,Oktober,Mrd,überhaupt,eigene,Dann,gegeben,Außerdem,Stunden,eigentlich,Meter,ließ,Probleme,vielleicht,ebenso,Bereich,zum Beispiel,Bis,Höhe,Familie,Während,Bild,Ländern,Informationen,Frankreich,Tagen,schwer,zuvor,Vor,genau,April,stellen,neu,erwartet,Hamburg,sicher,führen,Mal,Über,mehrere,Wirtschaft,Mio,Programm,offenbar,Hier,weiteren,natürlich,konnten,stark,Dezember,Juli,ganze,kommenden,Kunden,bekommen,eben,kleine,trotz,wirklich,Lage,Länder,leicht,gekommen,Spiel,laut,November,kurz,politische,führt,innerhalb,unsere,meint,immer wieder,Form,Münchner,AG,anders,ihres,völlig,beispielsweise,gute,bislang,August,Hand,jede,GmbH,Film,Minuten,erreicht,beide,Musik,Kritik,Mitte,Verfügung,Buch,dürfen,Unter,jeweils,einigen,Zum,Umsatz,spielen,Daten,welche,müßten,hieß,paar,nachdem,Kunst,Euro,gebracht,Problem,Noch,jeden,Ihre,Sprecher,recht,erneut,längst,europäischen,Sein,Eltern,Beginn,besteht,Seine,mindestens,machte,Jetzt,bietet,außerdem,Bürger,Trainer,bald,Deutsche,Schon,Fragen,klar,Durch,Seiten,gehören,Dort,erstmals,Februar,zeigen,Titel,Stück,größten,FDP,setzt,Wert,Frankfurter,Staat,möchte,daher,wolle,Bundesregierung,lediglich,Nacht,Krieg,Opfer,Tod,nimmt,Firma,zuletzt,Werk,hohen,leben,unter anderem,Dieser,Kirche,weiterhin,gebe,gestellt,Mitglieder,Rahmen,zweite,Paris,Situation,gefunden,Wochenende,internationalen,Wasser,Recht,sonst,stand,Hälfte,Möglichkeit,versucht,blieb,junge,Mehrheit,Straße,Sache,arbeiten,Monate,Mutter,berichtet,letzte,Gericht,wollten,Ihr,zwölf,zumindest,Wahl,genug,Weise,Vater,Bericht,amerikanischen,hoch,beginnt,Wort,obwohl,Kopf,spielt,Interesse,Westen,verloren,Preis,Erst,jedem,erreichen,setzen,spricht,früher,teilte,Landes,zudem,einzelnen,bereit,Blick,Druck,Bayern,Kilometer,gemeinsam,Bedeutung,Chance,Politiker,Dazu,Zwei,besten,Ansicht,endlich,Stelle,direkt,Beim,Bevölkerung,Viele,solchen,Alle,solle,jungen,Einsatz,richtig,größte,sofort,neuer,ehemaligen,unserer,dürfte,schaffen,Augen,Rußland,Internet,Allerdings,Raum,Mannschaft,neun,kamen,Ausstellung,Zeiten,Dem,einzige,meine,Nun,Verfahren,Angebot,Richtung,Projekt,niemand,Kampf,weder,tatsächlich,Personen,dpa,Heute,geführt,Gespräch,Kreis,Hamburger,Schule,guten,Hauptstadt,durchaus,Zusammenarbeit,darin,Amt,Schritt,meist,groß,zufolge,Sprache,Region,Punkte,Vergleich,genommen,gleichen,du,Ob,Soldaten,Universität,verschiedenen,Kollegen,neues,Bürgermeister,Angst,stellte,Sommer,danach,anderer,gesagt,Sicherheit,Macht,Bau,handelt,Folge,Bilder,lag,Osten,Handel,sprach,Aufgabe,Chef,frei,dennoch,DDR,hohe,Firmen,bzw,Koalition,Mädchen,Zur,entwickelt,fand,Diskussion,bringt,Deshalb,Hause,Gefahr,per,zugleich,früheren,dadurch,ganzen,abend,erzählt,Streit,Vergangenheit,Parteien,Verhandlungen,jedenfalls,gesehen,französischen,Trotz,darunter,Spieler,forderte,Beispiel,Meinung,wenigen,Publikum,sowohl,meinte,mag,Auto,Lösung,Boden,Einen,Präsidenten,hinaus,Zwar,verletzt,weltweit,Sohn,bevor,Peter,mußten,keiner,Produktion,Ort,braucht,Zusammenhang,Kind,Verein,sprechen,Aktien,gleichzeitig,London,sogenannten,Richter,geplant,Italien,Mittel,her,freilich,Mensch,großer,Bonner,wenige,öffentlichen,Unterstützung,dritten,nahm,Bundesrepublik,Arbeitsplätze,bedeutet,Feld,Dr.,Bank,oben,gesetzt,Ausland,Ministerpräsident,Vertreter,z.B.,jedes,ziehen,Parlament,berichtete,Dieses,China,aufgrund,Stellen,warum,Kindern,heraus,heutigen,Anteil,Herr,Öffentlichkeit,Abend,Selbst,Liebe,Neben,rechnen,fällt,New York,Industrie,WELT,Stuttgarter,wären,Vorjahr,Sicht,Idee,Banken,verlassen,Leiter,Bühne,insbesondere,offen,stets,Theater,ändern,entschieden,Staaten,Experten,Gesetz,Geschäft,Tochter,angesichts,gelten,Mehr,erwarten,läuft,fordert,Japan,Sieg,Ist,Stimmen,wählen,russischen,gewinnen,CSU,bieten,Nähe,jährlich,Bremen,Schüler,Rede,Funktion,Zuschauer,hingegen,anderes,Führung,Besucher,Drittel,Moskau,immerhin,Vorsitzende,Urteil,Schließlich,Kultur,betonte,mittlerweile,Saison,Konzept,suchen,Zahlen,Roman,Gewalt,Köln,gesamte,indem,EU,Stunde,ehemalige,Auftrag,entscheiden,genannt,tragen,Börse,langen,häufig,Chancen,Vor allem,Position,alt,Luft,Studenten,übernehmen,stärker,ohnehin,zeigte,geplanten,Reihe,darum,verhindern,begann,Medien,verkauft,Minister,wichtig,amerikanische,sah,gesamten,einst,verwendet,vorbei,Behörden,helfen,Folgen,bezeichnet";
	}
}