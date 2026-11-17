using System.Text;
using DhrMaes.WanderingWyvern.Core.Models;

namespace DhrMaes.WanderingWyvern.Core.Services;

/// <summary>
/// Pre-built narrative templates for creating engaging, branching D&D sessions with one click.
/// </summary>
public record SessionTemplateDefinition(
    string Id,
    string Name,
    string Description,
    string Icon,
    string DefaultScriptMarkdown,
    string DefaultOutlineMarkdown
);

public static class SessionFlowTemplates
{
    public const string ClassicForkId = "fork";
    public const string DungeonCrawlId = "dungeon";
    public const string InfiltrationHeistId = "heist";
    public const string BossShowdownId = "boss";
    public const string BlankCanvasId = "blank";

    public static readonly IReadOnlyList<SessionTemplateDefinition> All =
    [
        new(
            Id: ClassicForkId,
            Name: "Het Klassieke Kruispunt",
            Description: "Start bij een splitsing. Spelers kiezen tussen twee verschillende routes (bv. moeras vs wachttoren) die samenkomen bij een spannende climax.",
            Icon: "🔀",
            DefaultScriptMarkdown: @"```mermaid
flowchart TD
    S1[""🚪 Scène 1 — Het Kruispunt""] -->|Kies het linkerpad (Moeras)| S2[""🔍 Scène 2 — Het Vergeten Moeras""]
    S1 -->|Kies het rechterpad (Wachttoren)| S3[""🗣️ Scène 3 — De Verlaten Wachttoren""]
    S2 -->|Volg het oude stenen pad| S4[""⚔️ Scène 4 — De Oude Ruïne""]
    S3 -->|Beklim de heuvelkam| S4
    S4 -->|Na het gevecht| S5[""🏕️ Scène 5 — Kampvuur & Epiloog""]
    %% positions: S1:180,60;S2:40,220;S3:320,220;S4:180,380;S5:180,540
```

## 🚪 Scène 1 — Het Kruispunt
> Voor jullie splitst het pad zich in tweeën. Links voert een modderig spoor diep een mistig moeras in, waar het geluid van krekels plotseling verstomt. Rechts kronkelt een rotsachtig pad omhoog naar een oude, verweerde wachttoren die boven de bomen uitsteekt.

*Wat doen de spelers?*
- **Survival-check (DC 12):** Bij succes ontdekken ze verse sporen van goblins richting het moeras, maar ook rookpluimpjes bij de wachttoren.

## 🔍 Scène 2 — Het Vergeten Moeras
> De mist trekt om jullie enkels als het pad verdwijnt in drassige grond. Tussen de dode wilgen glinstert een oud ornament half verzonken in het slijk.

*Onderzoek van het moeras:*
- **Perception-check DC 13:** Ze zien de drijfzandplek op tijd.
- **Investigation-check DC 15:** Ze vinden een half verrotte tas met 25 gp en een geheimzinnige brief.

## 🗣️ Scène 3 — De Verlaten Wachttoren
> De wachttoren lijkt op het eerste gezicht verlaten, maar bij de ingang wappert een gerafelde banier. Bovenin klinkt een zacht gestommel en het schrapen van metaal.

*Ontmoeting bij de toren:*
- Hier schuilt een gewonde deserteur of reiziger. Met een **Persuasion-check (DC 11)** deelt hij informatie over de ruïne verderop.

## ⚔️ Scène 4 — De Oude Ruïne
> De bomen wijken uiteen voor de brokstukken van een eeuwenoude tempel. Te midden van het altaar staan bewakers met getrokken wapens gereed.

*Encounter:*
- 3x Bandits / Goblins die het heiligdom bewaken.
- Tactiek: Ze gebruiken de zuilen als halve dekking (+2 AC).

## 🏕️ Scène 5 — Kampvuur & Epiloog
> Met de bewakers verslagen daalt de rust weer neer. De zon zakt achter de heuvels en werpt lange schaduwen over de geopende schatkist.

- Rustmoment: Korte of lange rust.
- Verdeling van loot en geheimen voor de volgende sessie.
",
            DefaultOutlineMarkdown: @"## Doel van de sessie
De spelers bereiken het kruispunt en moeten kiezen welke route ze nemen naar de oude ruïne, waar een confrontatie wacht.

## Scènes
1. **Het Kruispunt:** Keuzemoment tussen het moeras en de wachttoren.
2. **Het Vergeten Moeras:** Verkenning, valkuilen en sporen.
3. **De Verlaten Wachttoren:** Sociale ontmoeting en informatie inwinnen.
4. **De Oude Ruïne:** Gevecht bij het altaar.
5. **Kampvuur & Epiloog:** Rust, buit en afronding.

## Benodigde NPCs/Locaties/Items
- Geen gekoppelde entiteiten.
"
        ),
        new(
            Id: DungeonCrawlId,
            Name: "De Dungeon Crawl",
            Description: "Een klassieke kerkerverkenning: van de verzegelde ingang, langs valstrikken en bewakers, tot de baaskamer met de schat.",
            Icon: "🏰",
            DefaultScriptMarkdown: @"```mermaid
flowchart TD
    S1[""🔍 Scène 1 — De Verzegelde Ingang""] -->|Open het portaal| S2[""🧩 Scène 2 — De Hal der Vallen""]
    S2 -->|Val omzeild of getriggerd| S3[""⚔️ Scène 3 — De Bewakerskamer""]
    S3 -->|Sleutel gevonden| S4[""⚔️ Scène 4 — De Baaskamer""]
    S4 -->|Overwinning| S5[""🏕️ Scène 5 — De Schatkamer & Uitgang""]
    %% positions: S1:180,60;S2:180,220;S3:180,380;S4:180,540;S5:180,700
```

## 🔍 Scène 1 — De Verzegelde Ingang
> Een massieve stenen poort bedekt met runen blokkeert de doorgang naar het ondergrondse complex. Uit de spleten stroomt een kille luchtstroom die naar ozon en aarde ruikt.

*Puzzel / Entree:*
- **Arcana of Athletics DC 13:** Om de runen te activeren of de hefboom te forceren.

## 🧩 Scène 2 — De Hal der Vallen
> De gang voor jullie is geplaveid met brede tegels. Op de muren zijn kleine openingen te zien ter hoogte van borsthoogte.

*Valstrik:*
- **Perception DC 14:** Om de drukplaten op de vloer op te merken.
- **Thieves' Tools DC 13:** Om het mechanisme onklaar te maken. Bij falen: pijlenval (1d6 piercing damage).

## ⚔️ Scène 3 — De Bewakerskamer
> Deze ronde zaal werd ooit gebruikt door de kasteelwachten. Verroeste wapenrekken staan tegen de muren en skeletten komen krakend overeind.

*Gevecht:*
- 4x Skeleton Warriors. Ze vallen aan zodra iemand het midden van de kamer bereikt.

## ⚔️ Scène 4 — De Baaskamer
> Aan het einde van een lange trap wacht een troonzaal. Een zware schaduw rijst op vanaf de troon, omringd door gloeiende magische fakkels.

*Baasgevecht:*
- De Kerkercommandant met unieke acties en legendarische weerstand.

## 🏕️ Scène 5 — De Schatkamer & Uitgang
> Achter de troon klikt een verborgen deur open. Een stenen kist glanst in het kaarslicht en een trap leidt omhoog naar de buitenlucht.

- Schat en beloningen: Magisch voorwerp en goudstukken.
",
            DefaultOutlineMarkdown: @"## Doel van de sessie
De spelers dringen de verzegelde kerker binnen, overwinnen de vallen en verslaan de kerkercommandant.

## Scènes
1. **De Verzegelde Ingang:** Deur openen met Arcana of kracht.
2. **De Hal der Vallen:** Puzzel en valkuil vermijden.
3. **De Bewakerskamer:** Gevecht met skeletwachten.
4. **De Baaskamer:** Climaxgevecht tegen de commandant.
5. **De Schatkamer:** Buit verzamelen en ontsnappen.

## Benodigde NPCs/Locaties/Items
- Geen gekoppelde entiteiten.
"
        ),
        new(
            Id: InfiltrationHeistId,
            Name: "De Infiltratie / Heist",
            Description: "Een missie met keuzes tussen sluipen en praten: binnendringen, het doelwit bemachtigen en ontsnappen.",
            Icon: "🕵️",
            DefaultScriptMarkdown: @"```mermaid
flowchart TD
    S1[""🚪 Scène 1 — De Briefing & Planning""] -->|Kies de achterdeur (Sluipen)| S2A[""🔍 Scène 2A — De Dienstingang""]
    S1 -->|Kies de voordeur (Social/Bluff)| S2B[""🗣️ Scène 2B — De Receptie & Feestzaal""]
    S2A -->|Binnen zonder alarm| S3[""🧩 Scène 3 — De Kluis & Het Doelwit""]
    S2B -->|Binnen met vermomming| S3
    S3 -->|Doelwit gepakt| S4[""⚔️ Scène 4 — Het Alarm & De Ontsnapping""]
    S4 -->|Ontsnapt| S5[""🏕️ Scène 5 — Het Onderduikadres""]
    %% positions: S1:180,60;S2A:40,220;S2B:320,220;S3:180,380;S4:180,540;S5:180,700
```

## 🚪 Scène 1 — De Briefing & Planning
> Boven een uitgespreide plattegrond op een krakende tafel bespreken jullie de laatste details van het plan. Het landhuis is zwaar bewaakt, maar er zijn twee manieren binnen: via de dienstingang achterom, of vermomd door de hoofdingang tijdens het bal.

## 🔍 Scène 2A — De Dienstingang
> In de schaduw van de keukenmuur wachten twee koks die even pauze nemen. Het venster naar de voorraadkamer staat op een kier.

*Stealth benadering:*
- **Stealth DC 13:** Onopgemerkt door het raam glippen.

## 🗣️ Scène 2B — De Receptie & Feestzaal
> Vioolmuziek en gelach vullen de marmeren hal. Rijk geklede edelen nippen aan wijn terwijl wachters nauwlettend toekijken.

*Sociale benadering:*
- **Deception of Persuasion DC 12:** Om de gastenlijst-controleur te misleiden.

## 🧩 Scène 3 — De Kluis & Het Doelwit
> Achter een schilderij in het privékantoor bevindt zich een massieve kluis met een ingenieus draaislot.

*Kluiskraak:*
- **Investigation / Thieves' Tools DC 14:** Om de juiste cijfercode te ontcijferen.

## ⚔️ Scène 4 — Het Alarm & De Ontsnapping
> Een luide bel weergalmt door de gangen! Zware laarzen haasten zich jullie richting uit.

*Achtervolging:*
- Skill challenge of gevecht met 3 wachters om het terrein levend te verlaten.

## 🏕️ Scène 5 — Het Onderduikadres
> Buiten adem en veilig achter gesloten deuren inspecteren jullie de buit.
",
            DefaultOutlineMarkdown: @"## Doel van de sessie
Het infiltreren van het landhuis om het doelwit uit de kluis te stelen en onopgemerkt te ontsnappen.

## Scènes
1. **De Briefing:** Keuze tussen stealth of vermomming.
2. **Infiltratie:** Via dienstingang of feestzaal.
3. **De Kluis:** Puzzel en diefstal van het document.
4. **Het Alarm:** Ontsnappingsgevecht of achtervolging.
5. **Het Onderduikadres:** Succesvolle afronding.

## Benodigde NPCs/Locaties/Items
- Geen gekoppelde entiteiten.
"
        ),
        new(
            Id: BossShowdownId,
            Name: "Boss Showdown",
            Description: "Een climax-sessie: benadering van het vijandelijke bolwerk, gevecht tegen de voorhoede, en de epische eindbaas.",
            Icon: "⚔️",
            DefaultScriptMarkdown: @"```mermaid
flowchart TD
    S1[""🔍 Scène 1 — De Nadering van het Bolwerk""] -->|Frontale aanval| S2A[""⚔️ Scène 2A — De Voorhoede""]
    S1 -->|Geheime bergpas| S2B[""🧩 Scène 2B — De Verrassingsaanval""]
    S2A -->|Wachtpost verslagen| S3[""⚔️ Scène 3 — De Eindbaas Confrontatie""]
    S2B -->|Voordeel behaald| S3
    S3 -->|Eindbaas verslagen| S4[""🏕️ Scène 4 — De Overwinning & Epiloog""]
    %% positions: S1:180,60;S2A:40,220;S2B:320,220;S3:180,380;S4:180,540
```

## 🔍 Scène 1 — De Nadering van het Bolwerk
> De lucht kleurt asgrauw naarmate jullie het fort naderen. Donder rommelt in de verte en de poorten zijn stevig vergrendeld.

## ⚔️ Scène 2A — De Voorhoede
> Wachters met zware kruisbogen openen het vuur vanaf de borstweringen!

## 🧩 Scène 2B — De Verrassingsaanval
> Via een smalle rotsspleet sluipen jullie ongezien achter de vijandelijke linies, wat Surprise oplevert in de eerste gevechtsronde.

## ⚔️ Scène 3 — De Eindbaas Confrontatie
> In het hart van de burcht staat jullie aartsvijand. Magische energie pulseert door de kamer als het gevecht op leven en dood begint.

## 🏕️ Scène 4 — De Overwinning & Epiloog
> De dreiging is afgewend. De rust keert terug over het land en een nieuw hoofdstuk breekt aan voor de helden.
",
            DefaultOutlineMarkdown: @"## Doel van de sessie
De confrontatie met en het verslaan van de eindbaas in het vijandelijke bolwerk.

## Scènes
1. **De Nadering:** Tactische keuze van aanval.
2. **De Voorhoede:** Eerste gevecht of verrassing.
3. **De Eindbaas:** Het grote gevecht.
4. **Epiloog:** Afwikkeling en beloning.

## Benodigde NPCs/Locaties/Items
- Geen gekoppelde entiteiten.
"
        ),
        new(
            Id: BlankCanvasId,
            Name: "Leeg Canvas",
            Description: "Start met een schone lei: één startscène klaar om direct met de knoppen en bouwblokken uit te breiden.",
            Icon: "📄",
            DefaultScriptMarkdown: @"```mermaid
flowchart TD
    S1[""🚪 Scène 1 — De Start""]
    %% positions: S1:180,60
```

## 🚪 Scène 1 — De Start
> Beschrijf hier de opening van de sessie voor de spelers...

*Wat kunnen de spelers doen?*
- Voeg hier checks en keuzes toe.
",
            DefaultOutlineMarkdown: @"## Doel van de sessie
Nieuwe sessie klaar voor invulling.

## Scènes
1. **De Start:** Openingsscène.

## Benodigde NPCs/Locaties/Items
- Geen gekoppelde entiteiten.
"
        )
    ];

    public static SessionTemplateDefinition GetTemplate(string? id)
    {
        return All.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase)) ?? All[0];
    }
}
