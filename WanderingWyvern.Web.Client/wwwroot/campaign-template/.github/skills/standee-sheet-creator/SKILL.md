---
name: standee-sheet-creator
description: Create consistent, print-ready D&D character standees (paper pawns/miniatures) for 3D-printed holders (16.6 mm slot width, 9 mm tab height) on a 2-page A4 landscape PDF with matching front and rear views.
---

# Skill: D&D Character Standee Sheet Creator

## Purpose

Create consistent, print-ready character standees for the D&D Campaign
project. The standees are printed on paper, optionally laminated, cut
out, and inserted into the user's 3D-printed holders.

The goal is to produce a clean, low-ink, double-sided A4 PDF with
matching front and back views, while avoiding unnecessary prompt
iterations.

## Design principles

1.  **The character is the product.** Do not add decorative page frames,
    ornate borders, background scenes, title banners, instructions,
    flourishes, or other artwork around the standees.
2.  **White background only.** Use a plain white page so the sheet is
    economical to print and easy to cut.
3.  **No wasted ink.** Keep colour and detail on the character itself.
    Avoid large filled areas, decorative panels, shadows on the page,
    and unnecessary gold ornamentation.
4.  **Preserve established character identity.** Use the
    supplied/reference portrait as the source of truth for face, age,
    hairstyle, build, clothing, accessories, colours, and overall
    personality. Do not redesign a character unless explicitly asked.
5.  **Full-body views.** Show the entire character, including shoes/feet
    and the complete outline of clothing, weapons, hair, hats, capes,
    and other accessories. Do not crop the head, hands, weapons, or hem.
6.  **Consistent collection style.** All characters on one sheet should
    share the same rendering style, lighting, detail level, scale
    convention, and clean cutout treatment.
7.  **Keep the character's natural silhouette.** A wide dress, cape, or
    weapon may extend beyond the width of the holder. Only the small
    insertion tab is constrained by the holder.

## Holder dimensions and insertion tab

The user's OpenSCAD holder has these relevant dimensions:

-   Base footprint: **25 mm × 10 mm**
-   Slot thickness: **0.5 mm**
-   Slot length: approximately **16.67 mm** (`stand_width * 2/3`)
-   Intended insertion depth: approximately **10 mm** (the holder's
    upright slot region)

Use these standee tab targets:

-   **Tab width:** 16.5--16.6 mm when laid out at final print size.
-   **Tab height:** approximately 9 mm.
-   **Tab thickness:** paper/lamination thickness has already been
    tested by the user and does not need to be modelled or questioned.
-   **Tab shape:** a simple, centered rectangular extension directly
    connected to the bottom of the character cutout.
-   **Tab label:** character name only, centered on the tab in small,
    legible type. A restrained dark serif font is suitable. A tiny
    divider may be used only if it does not make the tab crowded.
-   Do not add a separate pedestal, oversized blank base, decorative
    nameplate, or large white box beneath the character.

The tab must be part of the same continuous cutout silhouette as the
character. It is not a separate object.

## Sheet layout

Default output:

-   **Two-page PDF**
-   **A4 landscape**
-   Page 1: front views (recto)
-   Page 2: matching back views (verso)
-   Same character order on both pages
-   Same final physical scale and placement on both pages
-   Plain white page background
-   Comfortable spacing between cutouts so scissors can pass between
    them
-   Thin, unobtrusive cut contour around each standee; use a light
    neutral grey or dark grey hairline, not a decorative frame

For a small NPC collection, arrange characters in one horizontal row if
they fit legibly. If the characters become too small, use a balanced
grid while preserving identical positions between the front and back
pages.

Do not place page titles, collection labels, print instructions,
ornamental borders, or explanatory text on the print sheet. Put any
instructions in the chat response, not on the PDF.

## Front and back views

Each standee needs two corresponding views:

-   **Front:** the established character design, full body, facing
    mostly forward or in a slight three-quarter pose.
-   **Back:** a believable rear view of the same character, showing the
    back of the same outfit, hairstyle, cape, coat, dress, straps,
    belts, and accessories.

The back must not be a newly invented alternate costume. Maintain the
same clothing materials, colours, proportions, and accessories.

### Duplex alignment

The reverse page must be prepared for the intended printer duplex
setting. For A4 landscape, the expected setting in the current workflow
is **double-sided, flip on short edge**, with the reverse artwork
mirrored horizontally when required by the page assembly method.

Important: printer drivers vary. Do not claim alignment is guaranteed
solely because the page is mirrored. Keep identical scale, page
placement, and cutout centers on both sides. Recommend one plain-paper
test print before laminating the full sheet.

## Image-generation prompt requirements

When generating the artwork, explicitly state:

-   A clean, print-production character standee sheet on a pure white
    background.
-   The exact character count and names.
-   Full-body front views, with complete silhouettes and visible feet.
-   A matching back-view row showing the same characters in the same
    left-to-right order.
-   Each character isolated with generous white space between
    neighbouring cutouts.
-   A single continuous outer cut contour around each character and its
    integrated bottom tab.
-   A narrow bottom tab, approximately 16.6 mm wide by 9 mm high at
    final printed size.
-   The character name centered on the tab, and nowhere else.
-   No page decoration, no background environment, no decorative frame,
    no title, no extra labels, no instruction panel, no separate base,
    and no oversized white rectangles.
-   Consistent character identity and costume details based on the
    supplied references.

### Suggested reusable prompt

> Create a clean, print-production sheet of [NUMBER] tabletop RPG
> character standees on a pure white background. Use the supplied
> character reference images as strict identity and costume references.
> Each character must be shown full-body, with the complete head, hair,
> hands, weapons, clothing silhouette, and feet visible. Arrange the
> front views in one row, left to right in this exact order: [CHARACTER
> NAMES]. Below them, show matching full-body rear views in the exact
> same order, depicting the backs of the same outfits and hairstyles.
> Each figure must be an isolated cutout with generous white space
> around it and a thin, unobtrusive cut contour. At the bottom of each
> figure, continue the cutout directly into one small centered
> rectangular insertion tab, approximately 16.6 mm wide and 9 mm high at
> final A4 print size. Put only that character's name centered on the
> tab in small, legible dark serif lettering. The tab is part of the
> character cutout, not a separate pedestal. The character body may be
> any width; only the insertion tab must match the holder. Use a
> consistent cinematic Victorian-fantasy illustration style across the
> entire collection. Pure white page, minimal ink usage.
>
> **Strict exclusions:** no decorative page border, no ornate frame, no
> title banner, no headings, no instructions, no background scenery, no
> coloured page panels, no large gold decorations, no separate base, no
> oversized blank box under the feet, no extra text, and no cropped body
> parts.

### Important image-generation limitation

Image generators are useful for producing character artwork and visual
layouts, but they are not reliable tools for exact millimetre
dimensions, typographic accuracy, cut-line engineering, or duplex
registration. Do not rely on a generated image alone as the final
production PDF when precise physical sizing matters.

For best results:

1.  Generate or obtain each character's front and back artwork.
2.  Remove backgrounds and isolate the characters where needed.
3.  Assemble the sheet programmatically at the final physical
    dimensions.
4.  Add the tab geometry and name labels in the layout stage, not by
    asking the image generator to typeset them.
5.  Place the reverse images in matching positions and apply the
    required duplex mirroring.
6.  Export a two-page A4 landscape PDF.
7.  Verify the PDF page count, page dimensions, and tab width before
    delivery.

## Quality-control checklist

Before delivering the PDF, verify:

-   [ ] Exactly two pages: front and back.
-   [ ] A4 landscape page size.
-   [ ] Plain white background with no decorative page furniture.
-   [ ] Every character is full-body and not cropped.
-   [ ] Correct character identity and costume details.
-   [ ] Same character order on both pages.
-   [ ] Back views are genuine rear views of the same designs.
-   [ ] Each character has one integrated tab.
-   [ ] Tab width measures approximately 16.5--16.6 mm in the final PDF.
-   [ ] Tab height is approximately 9 mm.
-   [ ] Character name appears only on its tab and is readable.
-   [ ] No oversized white base rectangles or separate pedestals.
-   [ ] Cut contours are simple, visible, and easy to follow.
-   [ ] Front and back use the same scale and page placement.
-   [ ] Reverse page is prepared for landscape duplex printing, with
    mirroring checked.
-   [ ] A test print is recommended before laminating.

## Delivery notes

When presenting the result, include:

-   A direct PDF link.
-   A short note that the PDF has two pages (front and back).
-   Print guidance: A4 landscape, double-sided, flip on short edge
    (subject to printer test).
-   A reminder to test one sheet before laminating.

Do not claim exact duplex alignment is guaranteed without a physical
printer test.
