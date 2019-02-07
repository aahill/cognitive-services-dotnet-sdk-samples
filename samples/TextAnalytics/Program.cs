
using System;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TextAnalytics
{
    class Program
    {
        /// <summary>
        /// Container for subscription credentials. Replace this with your valid Text Analytics subscription key.
        private const string SubscriptionKey = "enter-your-key-here";

        /// </summary>
        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }


        /// <summary>
        /// Uses the Text Analytics client to perform sentiment analysis on sample documents.
        /// </summary>
        static void sentimentAnalysis(ITextAnalyticsClient client)
        {
            Console.WriteLine("\n\n===== SENTIMENT ANALYSIS ======");
            
            //The documents to be submitted for language detection. The ID can be any value.
            var inputDocuments = new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput("en", "0", "I had the best day of my life."),
                          new MultiLanguageInput("en", "1", "This was a waste of my time. The speaker put me to sleep."),
                          new MultiLanguageInput("es", "2", "No tengo dinero ni nada que dar..."),
                          new MultiLanguageInput("it", "3", "L'hotel veneziano era meraviglioso. È un bellissimo pezzo di architettura."),
                        });

            SentimentBatchResult result = client.SentimentAsync(inputDocuments).Result;


            // Print the sentiment results
            foreach (var document in result.Documents)
            {
                Console.WriteLine($"Document ID: {document.Id} , Sentiment Score: {document.Score:0.00}");
            }
        } 

        /// <summary>
        /// Uses the Text Analytics client to detect the language these sample documents are written in.
        /// </summary>
        static void detectLanguage(ITextAnalyticsClient client)
        {
            Console.WriteLine("===== LANGUAGE DETECTION ======");

            //The documents to be submitted for language detection. The ID can be any value.
            var inputDocuments = new BatchInput(
                    new List<Input>()
                        {
                          new Input("1", "This is a document written in English."),
                          new Input("2", "Este es un document escrito en Español."),
                          new Input("3", "这是一个用中文写的文件")
                    });

            var result = client.DetectLanguageAsync(inputDocuments).Result;

            // Print the language results.
            foreach (var document in result.Documents)
            {
                Console.WriteLine($"Document ID: {document.Id} , Language: {document.DetectedLanguages[0].Name}");
            }

        }

        /// <summary>
        /// Uses the Text Analytics client to extract the key phrases from sample documents.
        /// </summary>
        public static void keyPhraseExtraction(ITextAnalyticsClient client)
        {
            Console.WriteLine("\n\n===== KEY-PHRASE EXTRACTION ======");

            //The documents to be submitted for key-phrase extraction. The ID can be any value.
            var inputDocuments = new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput("ja", "1", "猫は幸せ"),
                          new MultiLanguageInput("de", "2", "Fahrt nach Stuttgart und dann zum Hotel zu Fu."),
                          new MultiLanguageInput("en", "3", "My cat might need to see a veterinarian."),
                          new MultiLanguageInput("es", "4", "A mi me encanta el fútbol!")
                        });

            KeyPhraseBatchResult result2 = client.KeyPhrasesAsync(inputDocuments).Result;

            // Print the key phrases that were extracted
            foreach (var document in result2.Documents)
            {
                Console.WriteLine($"Document ID: {document.Id} ");
                Console.WriteLine("\t Key phrases:");

                foreach (string keyphrase in document.KeyPhrases)
                {
                    Console.WriteLine($"\t\t{keyphrase}");
                }
            }

        }

        static void detectEntities(ITextAnalyticsClient client) { 
        
            Console.WriteLine("\n\n===== ENTITY DETECTION ======");

            //The documents to be submitted for entity detection.
            var inputDocuments = new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput("en", "0", "The Great Depression began in 1929. By 1933, the GDP in The United States fell by 25%.")
                        });

            EntitiesBatchResultV2dot1 result = client.EntitiesAsync(inputDocuments).Result;

            // Print the found entities
            foreach (var document in result.Documents)
            {
                Console.WriteLine($"Document ID: {document.Id} ");


                foreach (EntityRecordV2dot1 entity in document.Entities)
                {

                    Console.WriteLine($" Entity name: {entity.Name} \n Wikipedia URL: {entity.WikipediaUrl}");
                    Console.WriteLine($" Entity type: { entity.Type} \n Entity Sub-type: {entity.SubType}");
                    Console.WriteLine("---------------------");
                }
            }
        }

        static void Main(string[] args)
        {

            // Create a Text Analytics client.
            ITextAnalyticsClient client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
            {
                //Replace 'westus' with the correct region for your Text Analytics subscription
                Endpoint = "https://westus.api.cognitive.microsoft.com"
            }; 

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            detectLanguage(client);
            keyPhraseExtraction(client);
            sentimentAnalysis(client);
            detectEntities(client);

            Console.ReadLine();
        }
    }
}