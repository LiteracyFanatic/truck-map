open System
open System.IO
open Csv
open CoordinateSharp
open XPlot.Plotly
open Giraffe.ViewEngine

let wisconsinText = File.ReadAllLines("wisconsin")
let wisconsinCoordinates = 
    wisconsinText
    |> Array.toList
    |> List.map (fun line -> 
        let c = line.Split(", ")
        let coord = Coordinate(Double.Parse(c.[1]), Double.Parse(c.[0]))
        coord.Cartesian.X, coord.Cartesian.Y
    )

let cityText = File.ReadAllText("wisconsin-cities.csv")

let parseCity (l: ICsvLine) =
    let coord = Coordinate(Double.Parse(l.["lat"]), Double.Parse(l.["lng"]))
    l.["city"], (coord.Cartesian.X, coord.Cartesian.Y)

let cities =
    CsvReader.ReadFromText(cityText)
    |> Seq.map parseCity
    |> Map.ofSeq

let schoolsText = File.ReadAllText("schools.csv")

let parseSchool (l: ICsvLine) =
    let coord = Coordinate(Double.Parse(l.["lat"]), Double.Parse(l.["lng"]))
    l.["school"], (coord.Cartesian.X, coord.Cartesian.Y)

let schools =
    CsvReader.ReadFromText(schoolsText)
    |> Seq.map parseSchool
    |> Seq.toList

let schoolNames =
    schools
    |> List.map fst

let schoolCoordinates =
    schools
    |> List.map snd

let dealershipNames =
    [
        "Racine"
        "Milwaukee"
        "Sheboygan"
        "Fond du Lac"
        "Green Bay"
        "La Crosse"
        "Mauston"
        "Madison"
    ]

let dealershipCoordinates =
    dealershipNames
    |> List.map (fun n -> cities.[n])

let wisconsinTrace =
    Scatter(
        x=List.map fst wisconsinCoordinates,
        y=List.map snd wisconsinCoordinates,
        showlegend=false,
        line=Line(color="gray")
    )

let dealershipsTrace =
    Scatter(
        x=List.map fst dealershipCoordinates,
        y=List.map snd dealershipCoordinates,
        mode="markers+text",
        textposition = [
            "top right"
            "top right"
            "bottom center"
            "bottom left"
            "bottom center"
            "top right"
            "top right"
            "bottom right"
        ],
        textfont=Textfont(size=18., color="black"),
        text=dealershipNames,
        name="Dealerships",
        marker=Marker(size=10., symbol="diamond", color="black")
    )

let schoolsTrace =
    Scatter(
        x=List.map fst schoolCoordinates,
        y=List.map snd schoolCoordinates,
        mode="markers+text",
        textposition = [
            "top left"
            "top left"
            "top left"
            "top left"
            "top left"
            "top left"
            "top left"
            "bottom"
            "top right"
            "top left"
            "top left"
            "top left"
            "top left"
            "top left"
            "bottom left"
            "top left"
        ],
        textfont=Textfont(size=18., color="black"),
        text=[1 .. schoolNames.Length],
        name="Schools",
        marker=Marker(size=5., symbol="circle", color="black")
    )

let layout = 
    Layout(
        width=900.,
        height=900.,
        xaxis=Xaxis(showgrid=false, zeroline=false, showticklabels=false),
        yaxis=Yaxis(showgrid=false, zeroline=false, showticklabels=false),
        title="Kriete Group Locations and Technical Colleges of Wisconsin",
        titlefont=Font(size=24., color="black"),
        legend=Legend(font=Font(size=18., color="black"))
    )

let chart =
    [wisconsinTrace; dealershipsTrace; schoolsTrace]
    |> Chart.Plot
    |> Chart.WithLayout layout
    |> Chart.WithSize(900, 900)
    |> Chart.WithId "map"

let schoolList =
    ol [] (List.map (fun x -> li [] [ str x ]) schoolNames)

let css = """
g.pointtext {
display: none;
}

ol {
column-count: 3;
}

.modebar-container {
display: none;
}
"""

let page =
    html [] [
        head [] [
            script [_src "https://cdn.plot.ly/plotly-latest.min.js"] []
            style [] [rawText css]
        ]
        body [] [
            div [_id "map"] []
            schoolList
            rawText (chart.GetInlineJS())
        ]
    ]

File.WriteAllText("map.html", RenderView.AsString.htmlDocument page)
