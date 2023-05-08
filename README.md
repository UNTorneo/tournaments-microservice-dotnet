# tournaments-microservice-dotnet
API member of a microservices family for a tournament management application

Setting environment variables

setx ASPNETCORE_TournamentsMongoDB__ConnectionURI "mongodb+srv://danielfgalindoc:Mongocluster123*@cluster0.a1etllb.mongodb.net/?retryWrites=true&w=majority" /M

setx ASPNETCORE_TeamsMongoDB__ConnectionURI "mongodb+srv://danielfgalindoc:Mongocluster123*@cluster0.a1etllb.mongodb.net/?retryWrites=true&w=majority" /M

setx ASPNETCORE_MatchesMongoDB__ConnectionURI "mongodb+srv://danielfgalindoc:Mongocluster123*@cluster0.a1etllb.mongodb.net/?retryWrites=true&w=majority" /M

Building image
cd ..
docker build -f .\TournamentWebService\Dockerfile --force-rm -t tournamentcoreapp .

Running container
docker run -d -p 443:443 --name TournamentWebApp tournamentcoreapp


Setting db connectionStrings in user-secrets (In case env variables doesn't work)

dotnet user-secrets set "TournamentsMongoDB:ConnectionURI" "mongodb+srv://danielfgalindoc:Mongocluster123*@cluster0.a1etllb.mongodb.net/?retryWrites=true&w=majority"

dotnet user-secrets set "TeamsMongoDB:ConnectionURI" "mongodb+srv://danielfgalindoc:Mongocluster123*@cluster0.a1etllb.mongodb.net/?retryWrites=true&w=majority"

dotnet user-secrets set "MatchesMongoDB:ConnectionURI" "mongodb+srv://danielfgalindoc:Mongocluster123*@cluster0.a1etllb.mongodb.net/?retryWrites=true&w=majority"
