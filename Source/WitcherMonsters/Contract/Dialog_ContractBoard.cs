using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace WitcherMonsters
{
    public class Dialog_ContractBoard : Window
    {
        private const float RowPadding = 8f;
        private const float RowSpacing = 6f;
        private const float AcceptButtonWidth = 90f;
        private const float AcceptButtonMargin = 10f;
        private const float MinRowHeight = 100f;

        private readonly Caravan caravan;
        private readonly Settlement settlement;
        private readonly WorldObjectComp_ContractBoard board;
        private Vector2 scrollPosition;

        public override Vector2 InitialSize => new Vector2(620f, 580f);

        public Dialog_ContractBoard(Caravan caravan, Settlement settlement, WorldObjectComp_ContractBoard board)
        {
            this.caravan = caravan;
            this.settlement = settlement;
            this.board = board;
            forcePause = true;
            absorbInputAroundWindow = true;
            doCloseButton = true;
            doCloseX = true;
            closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, 0f, inRect.width, 35f), "Browse witcher contracts — " + settlement.LabelCap);
            Text.Font = GameFont.Small;

            float y = 40f;
            Widgets.Label(
                new Rect(0f, y, inRect.width, 40f),
                "Frightened locals describe attacks in vague, contradictory terms. None of them name the beast with certainty.");
            y += 45f;

            float bottomMargin = CloseButSize.y + 10f;
            float labelWidth = inRect.width - AcceptButtonWidth - AcceptButtonMargin - 24f;
            Rect scrollRect = new Rect(0f, y, inRect.width, inRect.height - y - bottomMargin);
            Rect viewRect = new Rect(0f, 0f, scrollRect.width - 16f, GetContentHeight(labelWidth));

            Widgets.BeginScrollView(scrollRect, ref scrollPosition, viewRect);
            float curY = 0f;

            if (board.Offers.Count == 0)
            {
                Widgets.Label(new Rect(0f, 0f, viewRect.width, 30f), "No contracts posted.");
            }
            else
            {
                foreach (ContractOffer offer in board.Offers)
                {
                    string summary = board.DescribeOffer(offer);
                    float rowHeight = GetRowHeight(summary, labelWidth);
                    Rect row = new Rect(0f, curY, viewRect.width, rowHeight);
                    Widgets.DrawBoxSolid(row, new Color(0.15f, 0.15f, 0.15f, 0.35f));
                    Widgets.Label(new Rect(10f, curY + 6f, labelWidth, rowHeight - 12f), summary);

                    float buttonY = curY + (rowHeight - 28f) * 0.5f;
                    if (Widgets.ButtonText(new Rect(row.width - AcceptButtonWidth - 4f, buttonY, AcceptButtonWidth, 28f), "Accept"))
                    {
                        Widgets.EndScrollView();
                        BeginAccept(offer);
                        return;
                    }

                    curY += rowHeight + RowSpacing;
                }
            }

            Widgets.EndScrollView();
        }

        private void BeginAccept(ContractOffer offer)
        {
            LongEventHandler.QueueLongEvent(
                () =>
                {
                    if (board.TryAccept(caravan, offer, out string failReason))
                    {
                        return;
                    }

                    if (!failReason.NullOrEmpty())
                    {
                        Messages.Message(failReason, MessageTypeDefOf.RejectInput);
                    }
                },
                "Generating contract...",
                doAsynchronously: false,
                exceptionHandler: null);
            Close();
        }

        private float GetRowHeight(string summary, float labelWidth)
        {
            float textHeight = Text.CalcHeight(summary, labelWidth);
            return Mathf.Max(MinRowHeight, textHeight + RowPadding + 8f);
        }

        private float GetContentHeight(float labelWidth)
        {
            if (board.Offers.Count == 0)
            {
                return 40f;
            }

            float height = 0f;
            foreach (ContractOffer offer in board.Offers)
            {
                height += GetRowHeight(board.DescribeOffer(offer), labelWidth) + RowSpacing;
            }

            return height;
        }
    }
}
