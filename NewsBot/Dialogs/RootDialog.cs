using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using NewsBot.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NewsBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
       
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (message.Text.ToLower() == "start")
            {
                PromptDialog.Choice(
                    context: context,
                    resume: GetNewsAsync,
                    options: NewsSources,
                    prompt: "Please Select Your Preferred News Source:",
                    retry: "Currently I support only one of the News Source among:.");
            }
            else
            {
                await context.PostAsync("Please Type Start to Begin");
            }
            
        }

        public List<string> NewsSources = new List<string>() {
            "bbc-news",
            "cnn",
            "associated-press"};
        
        public async Task GetNewsAsync(IDialogContext context, IAwaitable<string> result)
        {
            string newsSource = await result;
            var newsArticles =await GetNews(newsSource.ToString());
            if (newsArticles.Status == "ok")
            {
                List<Attachment> list = new List<Attachment>();
                foreach (Article articleResponse in newsArticles.Articles)
                {
                    var articleCard = new HeroCard()
                    {
                        Title = articleResponse.Title,
                        Subtitle = articleResponse.Author + ',' + articleResponse.PublishedAt,
                        Text = articleResponse.Description,
                        Images = new List<CardImage> { new CardImage(articleResponse.UrlToImage.AbsoluteUri) },
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "View Full Story", value: articleResponse.Url.ToString()) }
                    };
                    list.Add(articleCard.ToAttachment());
                }
                var reply = context.MakeMessage();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                reply.Attachments = list;
                await context.PostAsync(reply);
              
            }
            else
            {
                await context.PostAsync("An Error Occured Calling NewsAPI.");
            }
            PromptDialog.Choice(
                     context: context,
                     resume: GetNewsAsync,
                     options: NewsSources,
                     prompt: "Do you want more News?,Please Select Your Preferred News Source:",
                     retry: "Currently I support only one of the News Source among:.");

        }
        private async Task<LiveResponse> GetNews(string newsSource)
        {
            LiveResponse liveResponse = new LiveResponse();
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(string.Format("https://newsapi.org/v1/articles?source={0}&apiKey=94f312c927104585a9e98ebcfce44403", newsSource));
                var content = await response.Content.ReadAsStringAsync();
                liveResponse = JsonConvert.DeserializeObject<LiveResponse>(content);
                return liveResponse;
            }
            catch (Exception)
            {
               
                return liveResponse;
            }           
            
        }
    }
}