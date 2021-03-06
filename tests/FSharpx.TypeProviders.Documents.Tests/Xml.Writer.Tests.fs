﻿module FSharpx.TypeProviders.Tests.Xml.Writer.Tests

open NUnit.Framework
open FSharpx
open FsUnit

type AuthorsXml = StructuredXml<Schema="""<authors><author name="Ludwig" surname="Wittgenstein" age="29" isPhilosopher="True" size="30.3" /></authors>""">

[<Test>]
let ``Can set properties in inlined xml``() =
    let inlined = new AuthorsXml()
    let author = inlined.Root.GetAuthors() |> Seq.head

    author.Name <- "John"
    author.Name |> should equal "John"

    author.Age <- 30
    author.Age |> should equal 30

    author.IsPhilosopher <- false
    author.IsPhilosopher |> should equal false

    author.Size <- 42.42
    author.Size |> should equal 42.42

[<Test>]
let ``Can add author in inlined xml``() =
    let inlined = new AuthorsXml()

    let author = inlined.Root.NewAuthor()
    author.Name <- "John"
    author.Age <- 31
    author.IsPhilosopher <- false
    author.Size <- 22.2

    inlined.Root.AddAuthor author

    let authors = inlined.Root.GetAuthors() |> Seq.toList
    authors.Length |> should equal 2

[<Test>]
let ``Can use named parameters in author constructor``() =
    let inlined = new AuthorsXml()

    let author = inlined.Root.NewAuthor(Name="John", Age=31)
    author.Name |> should equal "John"    
    author.Age |> should equal 31

[<Test>]
let ``Can export modified xml``() = 
    let inlined = new AuthorsXml()
    let author = inlined.Root.GetAuthors() |> Seq.head

    author.Name <- "John"
    author.Age <- 31
    author.IsPhilosopher <- false
    author.Size <- 22.2

    inlined.Document.ToString(System.Xml.Linq.SaveOptions.DisableFormatting)
      .Replace("22,2","22.2")  // TODO: Use  InvariantCulture 
    |> should equal """<authors><author name="John" surname="Wittgenstein" age="31" isPhilosopher="False" size="22.2" /></authors>"""

[<Test>]
let ``Can serialize the xml``() =
    let inlined = new AuthorsXml()
    let xml = inlined.ToString()
    xml |> should equal "<authors>\r\n  <author name=\"Ludwig\" surname=\"Wittgenstein\" age=\"29\" isPhilosopher=\"True\" size=\"30.3\" />\r\n</authors>"

    
[<Test>]
let ``Can convert the xml to json``() =
    let inlined = new AuthorsXml()
    let json = inlined.ToJson()
    let expectedJson = """{"author":{"name":"Ludwig","surname":"Wittgenstein","age":"29","isPhilosopher":"True","size":"30.3"}}"""
    json.ToString() |> should equal expectedJson