using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using Intent.Packages.Constants;

namespace Intent.SoftwareFactory.Plugins.TagWeaving
{
    public class FileWeaver
    {
        private readonly ISoftwareFactoryEventDispatcher _eventDispatcher;
        public IProject Project { get; }
        public IFileMetaData FileMetaData { get; }

        public FileWeaver(ISoftwareFactoryEventDispatcher eventDispatcher, IProject project, IFileMetaData fileMetaData)
        {
            _eventDispatcher = eventDispatcher;
            this.Project = project;
            this.FileMetaData = fileMetaData;
        }


        public string Keep(TokenParser tokenParser, string generatedContent, string manualContent)
        {
            ContentParser p = new ContentParser(
                new List<TokenParser> { 
                    tokenParser
                }                    
                );

            var manualParse = p.Parse(manualContent);

            if (manualParse.Tokens.Count == 0)
            {
                return generatedContent;
            }
            if (manualParse.Tokens.Count % 2 != 0)
            {
                throw new Exception("Error Parsing for Embedding, invalid tags Manual");
            }

            var generatedParse = p.Parse(generatedContent);

            if (generatedParse.Tokens.Count == 0)
            {
                throw new Exception("Manual code found with no hook : " + ((TagToken)manualParse.Tokens[0]).Identifier);
            }
            if (generatedParse.Tokens.Count % 2 != 0)
            {
                throw new Exception("Error Parsing for Embedding, invalid tags Generated");
            }

            for (int im = 0; im < manualParse.Tokens.Count; im++)
            {
                if (im % 2 == 0)
                {
                    TagToken mToken = (TagToken)manualParse.Tokens[im];
                    string contentToInsert = manualParse.GetContentBetween(manualParse.Tokens[im], manualParse.Tokens[im + 1]);
                    TagToken catchToken = null;
                    bool foundManualHook = false;
                    for (int ig = 0; ig < generatedParse.Tokens.Count; ig++)
                    {
                        if (ig % 2 == 0)
                        {
                            TagToken gToken = (TagToken)generatedParse.Tokens[ig];
                            if (gToken.Identifier == mToken.Identifier)
                            {
                                foundManualHook = true;
                                generatedParse.ReplaceContentBetween(generatedParse.Tokens[ig], generatedParse.Tokens[ig + 1], contentToInsert);
                            }
                            if (gToken.Identifier == "catchAll" && catchToken == null)
                            {
                                catchToken = gToken;
                            }
                        }
                    }
                    if (!foundManualHook)
                    {
                        _eventDispatcher.Publish(SoftwareFactoryEvents.CodeWeaveCodeLossEvent, new Dictionary<string, string>
                                        {
                                            {"FullFileName", this.FileMetaData.GetFullLocationPathWithFileName()}
                                        });
                        if (catchToken == null)
                        {
                            // throw new Exception("Manual code found with no hook : " + mToken.Identifier);
                        }
                        else
                        {

                            generatedParse.AppendContent(contentToInsert, catchToken.Index + catchToken.Length + 1);
                        }
                    }

                }
            }

            return generatedParse.ToString();
        }

        public string Embed(TokenParser tokenParser, string generatedContent, string manualContent)
        {

            /* Parses things like
              
            //IntentManaged[void_SomeMethod]
             
            */
            ContentParser p = new ContentParser(
                new List<TokenParser> { 
                    tokenParser
                }
                );
            var manualParse = p.Parse(manualContent);

            if (manualParse.Tokens.Count == 0)
            {
                return manualContent;
            }
            if (manualParse.Tokens.Count % 2 != 0)
            {
                throw new Exception("Error Parsing for Embedding, invalid tags Manual");
            }

            var generatedParse = p.Parse(generatedContent);

            if (generatedParse.Tokens.Count == 0)
            {
                return manualContent;
            }
            if (generatedParse.Tokens.Count % 2 != 0)
            {
                throw new Exception("Error Parsing for Embedding, invalid tags Generated");
            }

            for (int ig = 0; ig < generatedParse.Tokens.Count; ig++)
            {
                if (ig % 2 == 0)
                {
                    TagToken gToken = (TagToken)generatedParse.Tokens[ig];
                    string contentToInsert = generatedParse.GetContentBetween(generatedParse.Tokens[ig], generatedParse.Tokens[ig + 1]);

                    for (int im = 0; im < manualParse.Tokens.Count; im++)
                    {
                        if (im % 2 == 0)
                        {
                            TagToken mToken = (TagToken)manualParse.Tokens[im];
                            if (mToken.Identifier == gToken.Identifier)
                            {
                                manualParse.ReplaceContentBetween(manualParse.Tokens[im], manualParse.Tokens[im + 1], contentToInsert);
                            }
                        }
                    }

                }
            }

            return manualParse.ToString();
        }
    }
}
