using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace TheBall.Core
{
    public static class CollectionUpdateImplementation
    {
        public static void Update_InvoiceSummaryContainer_OpenInvoices(InvoiceSummaryContainer invoiceSummaryContainer, InvoiceCollection localCollection, InvoiceCollection masterCollection)
        {
            throw new NotImplementedException();
        }

        public static void Update_InvoiceSummaryContainer_PredictedInvoices(InvoiceSummaryContainer invoiceSummaryContainer, InvoiceCollection localCollection, InvoiceCollection masterCollection)
        {
            throw new NotImplementedException();
        }

        public static void Update_InvoiceSummaryContainer_PaidInvoicesActiveYear(InvoiceSummaryContainer invoiceSummaryContainer, InvoiceCollection localCollection, InvoiceCollection masterCollection)
        {
            throw new NotImplementedException();
        }

        public static void Update_InvoiceSummaryContainer_PaidInvoicesLast12Months(InvoiceSummaryContainer invoiceSummaryContainer, InvoiceCollection localCollection, InvoiceCollection masterCollection)
        {
            throw new NotImplementedException();
        }

        public static void Update_InvoiceFiscalExportSummary_ExportedInvoices(InvoiceFiscalExportSummary invoiceFiscalExportSummary, InvoiceCollection localCollection, InvoiceCollection masterCollection)
        {
            throw new NotImplementedException();
        }
    }
}

namespace AaltoGlobalImpact.OIP
{
    public static class CollectionUpdateImplementation
    {

        internal static void Update_AccountModule_LocationCollection(AccountModule accountModule, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_LocationContainer_Locations(LocationContainer locationContainer, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_GroupContainer_LocationCollection(GroupContainer groupContainer, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            if (localCollection == null)
            {
                groupContainer.LocationCollection = AddressAndLocationCollection.CreateDefault();
                localCollection = groupContainer.LocationCollection;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        private static void appendMarkerLink(StringBuilder strBuilder, ReferenceToInformation referenceToInformation)
        {
            if (referenceToInformation == null || referenceToInformation.URL == null || referenceToInformation.Title == null)
                return;
            strBuilder.AppendFormat("<a class=\"oipmapmarkerlink\" href=\"javascript:void(0)\" onclick=\"OipOpenArticle(\\'{0}\\');\" >{1}</a><br>",
                                    referenceToInformation.URL, referenceToInformation.Title.Replace("'", ""));
            //strBuilder.AppendFormat("<a class=\"oipmapmarkerlink\" href=\"javascript:void(0)\">{1}</a><br>",
            //                        referenceToInformation.URL, referenceToInformation.Title.Replace("'", ""));
        }

        internal static void Update_Group_CategoryCollection(Group group, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            if (localCollection == null)
            {
                group.CategoryCollection = CategoryCollection.CreateDefault();
                localCollection = group.CategoryCollection;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        internal static void Update_CategoryContainer_Categories(CategoryContainer categoryContainer, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            if (localCollection == null)
            {
                categoryContainer.Categories = CategoryCollection.CreateDefault();
                localCollection = categoryContainer.Categories;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }

        const string NodeSourceTypeBlog = "BLOG";
        const string NodeSourceTypeActivity = "ACTIVITY";
        const string NodeSourceTypeTextContent = "TEXTCONTENT";
        const string NodeSourceTypeCategory = "CATEGORY";
        const string NodeSourceTypeLinkToContent = "LINKTOCONTENT";
        const string NodeSourceTypeEmbeddedContent = "EMBEDDEDCONTENT";
        const string NodeSourceTypeImage = "IMAGE";
        const string NodeSourceTypeBinaryFile = "BINARYFILE";
        
        private static void cleanUpRenderedNodes(RenderedNodeCollection nodes)
        {
            nodes.CollectionContent.RemoveAll(node => node.IsCategoryFilteringNode == false && node.ActualContentUrl == null);
            nodes.CollectionContent.RemoveAll(node => node.IsCategoryFilteringNode == false && (node.Categories.CollectionContent == null ||
                                                      node.Categories.CollectionContent.Count == 0));
            // Note the node1 and node2 are opposite parameters, because we want descending sort
            nodes.CollectionContent.Sort((node1, node2) => String.CompareOrdinal(node2.MainSortableText, node1.MainSortableText));
            foreach (var node in nodes.CollectionContent)
            {
                if (node.OpenNodeTitle == null)
                    node.OpenNodeTitle = node.Title;
            }
        }

        private static string getTitleOrNameFromCategory(Category category)
        {
            if (String.IsNullOrEmpty(category.Title))
                return category.CategoryName;
            return category.Title;
        }

        internal static ShortTextObject getShortTextObject(string source)
        {
            ShortTextObject shortText = ShortTextObject.CreateDefault();
            shortText.Content = source;
            return shortText;
        }

        internal static IEnumerable<ShortTextObject> getCategoryCollectionTexts(CategoryCollection categoryCollection, Func<Category, string> categoryToStringFunc)
        {
            return categoryCollection.GetIDSelectedArray()
                                     .Select(category =>
                                         {
                                             var textShort = ShortTextObject.CreateDefault();
                                             //textShort.Content = category.CategoryName;
                                             textShort.Content = categoryToStringFunc(category);
                                             return textShort;
                                         })
                                     .OrderBy(text => text.Content);
        }

        internal static IEnumerable<ShortTextObject> getLocationCollectionTexts(AddressAndLocationCollection locationCollection)
        {
            return locationCollection.GetIDSelectedArray().OrderBy(location => location.Location.LocationName)
                                     .Select(location =>
                                         {
                                             var textShort = ShortTextObject.CreateDefault();
                                             textShort.Content = location.ID;
                                             return textShort;
                                         });
        }

        private static string getTimeStampText(DateTime dateTime)
        {
            if (dateTime.TimeOfDay == TimeSpan.Zero)
                return dateTime.ToString("D");
            return dateTime.ToString("f");
        }
        /*
        internal static void Update_Category_ParentCategories(Category category, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            if (localCollection == null)
            {
                category.ParentCategories = CategoryCollection.CreateDefault();
                localCollection = category.ParentCategories;
            }
            localCollection.CollectionContent = masterCollection.CollectionContent;
            if (localCollection.OrderFilterIDList == null)
                localCollection.OrderFilterIDList = new List<string>();
        }*/

        internal static void Update_TextContent_Locations(TextContent textContent, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            // TODO: Remove objects, that are no longer available in master
        }

        internal static void Update_TextContent_Categories(TextContent textContent, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            // TODO: Remove objects, that are no longer available in master
        }

        internal static void Update_NodeSummaryContainer_NodeSourceCategories(NodeSummaryContainer nodeSummaryContainer, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            var nodes = nodeSummaryContainer.Nodes;
            nodes.CollectionContent.RemoveAll(node => node.TechnicalSource == NodeSourceTypeCategory);
            /*
            var usedParentCategoryIDs =
                masterCollection.CollectionContent.Where(cat => cat.ParentCategory != null)
                                .Select(cat => cat.ParentCategory.ID)
                                .ToArray();
            var nodeCategories =
                masterCollection.CollectionContent.Where(cat => usedParentCategoryIDs.Contains(cat.ID)).ToArray();
             * */
            var nodeCategories = masterCollection.CollectionContent.Where(cat => cat.ImageData != null).ToArray();
            var categoryNodes = nodeCategories.Select(getNodeFromCategory).ToArray();
            //var categoryNodes = masterCollection.CollectionContent.Select(getNodeFromCategory).ToArray();
            nodes.CollectionContent.AddRange(categoryNodes);
            cleanUpRenderedNodes(nodes);
            if(localCollection == null)
                localCollection = new CategoryCollection();
            localCollection.CollectionContent = masterCollection.CollectionContent;
            localCollection.RefreshOrderAndFilterListFromContent();
            
            // Sort categories based on the collection
            var flattenedArray = nodeSummaryContainer.NodeSourceCategories.SelectedIDCommaSeparated.Split(',');
            string commaSeparatedIDs = String.Join(",", flattenedArray);
            nodeSummaryContainer.NodeSourceCategories.SelectedIDCommaSeparated = commaSeparatedIDs;
            var newList =
                nodeSummaryContainer.NodeSourceCategories.CollectionContent.OrderBy(
                    cat => Array.IndexOf(flattenedArray, cat.ID)).ToList();
            nodeSummaryContainer.NodeSourceCategories.CollectionContent = newList;

        }

        internal static RenderedNode getNodeFromCategory(Category category)
        {
            RenderedNode node = RenderedNode.CreateDefault();
            node.OriginalContentID = category.ID;
            node.TechnicalSource = NodeSourceTypeCategory;
            node.Title = category.Title;
            node.Excerpt = category.Excerpt;
            if (category.ImageData != null)
            {
                node.ImageBaseUrl = category.ImageData.ContentUrlBase;
                node.ImageExt = category.ImageData.AdditionalFormatFileExt;
            }
            //node.ActualContentUrl = "../" + textContent.SemanticDomainName + "/" + textContent.Name + "/" + textContent.ID;
            if (category.ParentCategory != null)
            {
                node.Categories.CollectionContent.Add(getShortTextObject(category.ParentCategory.Title));
                node.CategoryNames.CollectionContent.Add(getShortTextObject(category.ParentCategory.CategoryName));
            }
            if (category.ParentCategoryID != null)
            {
                node.CategoryIDList = category.ParentCategoryID;
            }
            node.IsCategoryFilteringNode = true;
            node.CategoryFilters.CollectionContent.Add(getShortTextObject(category.CategoryName));
            //if (textContent.Locations != null)
            //    node.Locations.CollectionContent.AddRange(getLocationCollectionTexts(textContent.Locations));
            //node.Authors.CollectionContent.Add(getShortTextObject(textContent.Author));
            //node.MainSortableText = textContent.SortOrderNumber.ToString();
            return node;
        }

        internal static void Update_NodeSummaryContainer_NodeSourceEmbeddedContent(NodeSummaryContainer nodeSummaryContainer, EmbeddedContentCollection localCollection, EmbeddedContentCollection masterCollection)
        {
            var nodes = nodeSummaryContainer.Nodes;
            nodes.CollectionContent.RemoveAll(node => node.TechnicalSource == NodeSourceTypeEmbeddedContent);
            var embeddedContentNodes = masterCollection.CollectionContent.Select(getNodeFromEmbeddedContent).ToArray();
            nodes.CollectionContent.AddRange(embeddedContentNodes);
            cleanUpRenderedNodes(nodes);

        }

        internal static RenderedNode getNodeFromEmbeddedContent(EmbeddedContent embeddedContent)
        {
            RenderedNode node = RenderedNode.CreateDefault();
            node.OriginalContentID = embeddedContent.ID;
            node.TechnicalSource = NodeSourceTypeEmbeddedContent;
            node.Title = embeddedContent.Title;
            node.Excerpt = embeddedContent.Description;
            node.ActualContentUrl = embeddedContent.IFrameTagContents;
            if (embeddedContent.Categories != null)
            {
                node.Categories.CollectionContent.AddRange(getCategoryCollectionTexts(embeddedContent.Categories, getTitleOrNameFromCategory));
                node.CategoryNames.CollectionContent.AddRange(getCategoryCollectionTexts(embeddedContent.Categories, cat => cat.CategoryName));
                node.CategoryIDList =
                    String.Join(",", embeddedContent.Categories.CollectionContent.Select(cat => cat.ID).ToArray());
            }
            if (embeddedContent.Locations != null)
                node.Locations.CollectionContent.AddRange(getLocationCollectionTexts(embeddedContent.Locations));
            return node;
        }


        internal static void Update_NodeSummaryContainer_NodeSourceLinkToContent(NodeSummaryContainer nodeSummaryContainer, LinkToContentCollection localCollection, LinkToContentCollection masterCollection)
        {
            var nodes = nodeSummaryContainer.Nodes;
            nodes.CollectionContent.RemoveAll(node => node.TechnicalSource == NodeSourceTypeLinkToContent);
            var linkToContentNodes = masterCollection.CollectionContent.Select(getNodeFromLinkToContent).ToArray();
            nodes.CollectionContent.AddRange(linkToContentNodes);
            cleanUpRenderedNodes(nodes);
        }

        internal static RenderedNode getNodeFromLinkToContent(LinkToContent linkToContent)
        {
            RenderedNode node = RenderedNode.CreateDefault();
            node.OriginalContentID = linkToContent.ID;
            node.TechnicalSource = NodeSourceTypeLinkToContent;
            node.Title = linkToContent.Title;
            node.Excerpt = linkToContent.Description;
            if (linkToContent.ImageData != null)
            {
                node.ImageBaseUrl = linkToContent.ImageData.ContentUrlBase;
                node.ImageExt = linkToContent.ImageData.AdditionalFormatFileExt;
            }
            node.ActualContentUrl = linkToContent.URL;
            if (linkToContent.Categories != null)
            {
                node.Categories.CollectionContent.AddRange(getCategoryCollectionTexts(linkToContent.Categories, getTitleOrNameFromCategory));
                node.CategoryNames.CollectionContent.AddRange(getCategoryCollectionTexts(linkToContent.Categories, cat => cat.CategoryName));
                node.CategoryIDList =
                    String.Join(",", linkToContent.Categories.CollectionContent.Select(cat => cat.ID).ToArray());
            }
            if (linkToContent.Locations != null)
                node.Locations.CollectionContent.AddRange(getLocationCollectionTexts(linkToContent.Locations));
            return node;
        }

        internal static void Update_NodeSummaryContainer_NodeSourceImages(NodeSummaryContainer nodeSummaryContainer, ImageCollection localCollection, ImageCollection masterCollection)
        {
            var nodes = nodeSummaryContainer.Nodes;
            nodes.CollectionContent.RemoveAll(node => node.TechnicalSource == NodeSourceTypeImage);
            var imageNodes = masterCollection.CollectionContent.Select(getNodeFromImage).ToArray();
            nodes.CollectionContent.AddRange(imageNodes);
            cleanUpRenderedNodes(nodes);
        }

        internal static RenderedNode getNodeFromImage(Image image)
        {
            RenderedNode node = RenderedNode.CreateDefault();
            node.OriginalContentID = image.ID;
            node.TechnicalSource = NodeSourceTypeImage;
            node.Title = image.Title;
            if (!String.IsNullOrEmpty(image.Description))
                node.Excerpt = image.Description;
            else
                node.Excerpt = image.Caption;
            if (image.ImageData != null)
            {
                node.ImageBaseUrl = image.ImageData.ContentUrlBase;
                node.ImageExt = image.ImageData.AdditionalFormatFileExt;
                node.ActualContentUrl = image.ImageData.ContentUrl;
            }
            if (image.Categories != null)
            {
                node.Categories.CollectionContent.AddRange(getCategoryCollectionTexts(image.Categories, getTitleOrNameFromCategory));
                node.CategoryNames.CollectionContent.AddRange(getCategoryCollectionTexts(image.Categories, cat => cat.CategoryName));
                node.CategoryIDList =
                    String.Join(",", image.Categories.CollectionContent.Select(cat => cat.ID).ToArray());
            }
            if (image.Locations != null)
                node.Locations.CollectionContent.AddRange(getLocationCollectionTexts(image.Locations));
            return node;
        }


        internal static void Update_NodeSummaryContainer_NodeSourceBinaryFiles(NodeSummaryContainer nodeSummaryContainer, BinaryFileCollection localCollection, BinaryFileCollection masterCollection)
        {
            var nodes = nodeSummaryContainer.Nodes;
            nodes.CollectionContent.RemoveAll(node => node.TechnicalSource == NodeSourceTypeBinaryFile);
            var binaryFileNodes = masterCollection.CollectionContent.Select(getNodeFromBinaryFile).ToArray();
            nodes.CollectionContent.AddRange(binaryFileNodes);
            cleanUpRenderedNodes(nodes);
        }

        internal static RenderedNode getNodeFromBinaryFile(BinaryFile binaryFile)
        {
            RenderedNode node = RenderedNode.CreateDefault();
            node.OriginalContentID = binaryFile.ID;
            node.TechnicalSource = NodeSourceTypeBinaryFile;
            node.Title = binaryFile.Title;
            node.Excerpt = binaryFile.Description;
            node.ActualContentUrl = binaryFile.Data != null ? binaryFile.Data.ContentUrl : null;
            if (binaryFile.Categories != null)
            {
                node.Categories.CollectionContent.AddRange(getCategoryCollectionTexts(binaryFile.Categories, getTitleOrNameFromCategory));
                node.CategoryNames.CollectionContent.AddRange(getCategoryCollectionTexts(binaryFile.Categories, cat => cat.CategoryName));
                node.CategoryIDList =
                    String.Join(",", binaryFile.Categories.CollectionContent.Select(cat => cat.ID).ToArray());
            }
            return node;
        }


        internal static void Update_NodeSummaryContainer_NodeSourceTextContent(NodeSummaryContainer nodeSummaryContainer, TextContentCollection localCollection, TextContentCollection masterCollection)
        {
            var nodes = nodeSummaryContainer.Nodes;
            nodes.CollectionContent.RemoveAll(node => node.TechnicalSource == NodeSourceTypeTextContent);
            var textContentNodes = masterCollection.CollectionContent.Select(getNodeFromTextContent).ToArray();
            nodes.CollectionContent.AddRange(textContentNodes);
            cleanUpRenderedNodes(nodes);
        }

        internal static RenderedNode getNodeFromTextContent(TextContent textContent)
        {
            RenderedNode node = RenderedNode.CreateDefault();
            node.OriginalContentID = textContent.ID;
            node.TechnicalSource = NodeSourceTypeTextContent;
            node.Title = textContent.Title;
            node.OpenNodeTitle = textContent.OpenArticleTitle;
            node.Excerpt = textContent.Excerpt;
            if (textContent.ImageData != null)
            {
                node.ImageBaseUrl = textContent.ImageData.ContentUrlBase;
                node.ImageExt = textContent.ImageData.AdditionalFormatFileExt;
            }
            node.ActualContentUrl = "../" + textContent.SemanticDomainName + "/" + textContent.Name + "/" + textContent.ID;
            if (textContent.Categories != null)
            {
                node.Categories.CollectionContent.AddRange(getCategoryCollectionTexts(textContent.Categories, getTitleOrNameFromCategory));
                node.CategoryNames.CollectionContent.AddRange(getCategoryCollectionTexts(textContent.Categories, cat => cat.CategoryName));
                node.CategoryIDList =
                    String.Join(",", textContent.Categories.CollectionContent.Select(cat => cat.ID).ToArray());
            }
            if(textContent.Locations != null)
                node.Locations.CollectionContent.AddRange(getLocationCollectionTexts(textContent.Locations));
            node.Authors.CollectionContent.Add(getShortTextObject(textContent.Author));
            //node.MainSortableText = textContent.SortOrderNumber.ToString();
            node.MainSortableText = textContent.Published.ToString("s");
            node.TimestampText = getTimeStampText(textContent.Published);
            return node;
        }


        internal static void Update_LinkToContent_Locations(LinkToContent linkToContent, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            // TODO: Remove objects, that are no longer available in master
        }

        internal static void Update_LinkToContent_Categories(LinkToContent linkToContent, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            // TODO: Remove objects, that are no longer available in master
        }


        internal static void Update_EmbeddedContent_Locations(EmbeddedContent embeddedContent, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            // TODO: Remove objects, that are no longer available in master
        }

        internal static void Update_EmbeddedContent_Categories(EmbeddedContent embeddedContent, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            // TODO: Remove objects, that are no longer available in master
        }

        internal static void Update_Image_Locations(Image image, AddressAndLocationCollection localCollection, AddressAndLocationCollection masterCollection)
        {
            // TODO: Remove objects, that are no longer available in master
        }

        internal static void Update_Image_Categories(Image image, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            // TODO: Remove objects, that are no longer available in master
        }

        internal static void Update_BinaryFile_Categories(BinaryFile binaryFile, CategoryCollection localCollection, CategoryCollection masterCollection)
        {
            // TODO: Remove objects, that are no longer available in master
        }

    }
}