using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace PizzariaHelloWorld.Dialogs
{
    [Serializable]
    public class QnaDialogs : QnAMakerDialog
    {
        public QnaDialogs(): 
            base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnaSubscriptionKey"], ConfigurationManager.AppSettings["QnaKnowledgebaseId"], "Não encontrei sua resposta", 0.5)))
        {
            
        }

        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            var primeiraResposta = result.Answers.First().Answer;
            Activity resposta = ((Activity) context.Activity).CreateReply();

            var dadosResposta = primeiraResposta.Split(';');

            if (dadosResposta.Length == 1)
            {
                await context.PostAsync(primeiraResposta);
                return;
            }

            var titulo = dadosResposta[0];

            var imageUrl = dadosResposta[1];

            HeroCard card = new HeroCard
            {
                Title = titulo,
                Subtitle = "Parece delicioso"
            };

            card.Buttons = new List<CardAction>
            {
                new CardAction(ActionTypes.OpenUrl, "Veja que linda", value:imageUrl)
            };

            card.Images = new List<CardImage>
            {
                new CardImage(imageUrl, titulo)
            };

            resposta.Attachments.Add(card.ToAttachment());

            await context.PostAsync(resposta);
        }
    }
}