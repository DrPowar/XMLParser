using BenchmarkDotNet.Attributes;
using System.Collections.Immutable;

namespace ConsoleApp2.Entities
{
    public sealed record Book
    {
        public uint Id { get; set; }

        public string? Title { get; set; }

        public string? Genre { get; set; }

        public string? Author { get; set; }

        //Тут uint тому, що вхідні дані завжди містять тільки рік і DateTime не вийде зпарсити
        public uint PublicationDate { get; set; }

        public List<Chapter> Chapters { get; set; } = new List<Chapter>();

        public override string ToString()
        {
            return $"Id: {Id}, Title: {Title}, Genre: {Genre}, Author: {Author}, Publication Date: {PublicationDate}, Chapters Count: {Chapters.Count}";
        }

    }
}

//< Library >
//  < Books >
//    < Book id = "1" genre = "fiction" >
//      < Title > The Great Gatsby</Title>
//      <Author>F.Scott Fitzgerald</Author>
//      <PublicationDate>1925</PublicationDate>
//      <Chapters>
//        <Chapter number = "1" >
//          < Title > Chapter One</Title>
//          <Content>This is the content of chapter one...</Content>
//        </Chapter>
//        <Chapter number = "2" >
//          < Title > Chapter Two</Title>
//          <Content>This is the content of chapter two...</Content>
//        </Chapter>
//      </Chapters>
//    </Book>
//    <Book id = "2" genre= "fiction" >
//      < Title > To Kill a Mockingbird</Title>
//      <Author>Harper Lee</Author>
//      <PublicationDate>1960</PublicationDate>
//      <Chapters>
//        <Chapter number = "1" >
//          < Title > Chapter One</Title>
//          <Content>This is the content of chapter one...</Content>
//        </Chapter>
//        <Chapter number = "2" >
//          < Title > Chapter Two</Title>
//          <Content>This is the content of chapter two...</Content>
//        </Chapter>
//      </Chapters>
//    </Book>
//    <Book id = "3" genre= "fiction" >
//      < Title > 1984 </ Title >
//      < Author > George Orwell</Author>
//      <PublicationDate>1949</PublicationDate>
//      <Chapters>
//        <Chapter number = "1" >
//          < Title > Chapter One</Title>
//          <Content>This is the content of chapter one...</Content>
//        </Chapter>
//        <Chapter number = "2" >
//          < Title > Chapter Two</Title>
//          <Content>This is the content of chapter two...</Content>
//        </Chapter>
//        <Chapter number = "3" >
//          < Title > Chapter Three</Title>
//          <Content>This is the content of chapter three...</Content>
//        </Chapter>
//        <Chapter number = "4" >
//          < Title > Chapter Four</Title>
//          <Content>This is the content of chapter four...</Content>
//        </Chapter>
//        <Chapter number = "5" >
//          < Title > Chapter Five</Title>
//          <Content>This is the content of chapter five...</Content>
//        </Chapter>
//      </Chapters>
//    </Book>
//    <Book id = "4" genre= "fantasy" >
//      < Title > Harry Potter and the Sorcerer's Stone</Title>
//      <Author>J.K.Rowling</Author>
//      <PublicationDate>1997</PublicationDate>
//      <Chapters>
//        <Chapter number = "1" >
//          < Title > Chapter One</Title>
//          <Content>This is the content of chapter one...</Content>
//        </Chapter>
//        <Chapter number = "2" >
//          < Title > Chapter Two</Title>
//          <Content>This is the content of chapter two...</Content>
//        </Chapter>
//        <Chapter number = "3" >
//          < Title > Chapter Three</Title>
//          <Content>This is the content of chapter three...</Content>
//        </Chapter>
//        <Chapter number = "4" >
//          < Title > Chapter Four</Title>
//          <Content>This is the content of chapter four...</Content>
//        </Chapter>
//        <Chapter number = "5" >
//          < Title > Chapter Five</Title>
//          <Content>This is the content of chapter five...</Content>
//        </Chapter>
//        <Chapter number = "6" >
//          < Title > Chapter Six</Title>
//          <Content>This is the content of chapter six...</Content>
//        </Chapter>
//        <Chapter number = "7" >
//          < Title > Chapter Seven</Title>
//          <Content>This is the content of chapter seven...</Content>
//        </Chapter>
//      </Chapters>
//    </Book>
//    <Book id = "5" genre= "science fiction" >
//      < Title > Dune </ Title >
//      < Author > Frank Herbert</Author>
//      <PublicationDate>1965</PublicationDate>
//      <Chapters>
//        <Chapter number = "1" >
//          < Title > Chapter One</Title>
//          <Content>This is the content of chapter one...</Content>
//        </Chapter>
//        <Chapter number = "2" >
//          < Title > Chapter Two</Title>
//          <Content>This is the content of chapter two...</Content>
//        </Chapter>
//        <Chapter number = "3" >
//          < Title > Chapter Three</Title>
//          <Content>This is the content of chapter three...</Content>
//        </Chapter>
//      </Chapters>
//    </Book>
//  </Books>
//  <Members>
//    <Member id = "1001" >
//      < Name > John Doe</Name>
//      <MembershipDate>2021-01-15</MembershipDate>
//      <BooksBorrowed>
//        <Book id = "1" dueDate= "2021-02-15" />
//      </ BooksBorrowed >
//    </ Member >
//  </ Members >
//</ Library >
