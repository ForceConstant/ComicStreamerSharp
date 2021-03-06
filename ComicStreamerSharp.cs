﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp.Portable;
using Newtonsoft.Json.Linq;
using RestSharp.Portable.HttpClient;

namespace ComicStreamer
{
    public class ComicStreamerSharp
    {
        public ComicStreamerSharp(string asBaseUrl)
        {
            msBaseUrl = asBaseUrl;
        }

        private async Task<IRestResponse> ExecuteCommand(string Resource)
        {
            var client = new RestClient(msBaseUrl);

            var request = new RestRequest();
            request.Resource = Resource;

            IRestResponse response = await client.Execute(request);

            System.Diagnostics.Debug.Assert(response.IsSuccess,
                "Error retrieving response for " + Resource);

            return response;
        }
        
        public async Task<string> GetDBInfo()
        {
            IRestResponse response = await ExecuteCommand("dbinfo");

            return response.Content;
        }

        public async Task<string> GetVersion()
        {
            IRestResponse response = await ExecuteCommand("version");

            return response.Content;
        }

        public async Task<string> GetDeletedComics(string date)
        {
            /*
                  * /deleted
             - list of comic IDs that have been removed from the DB
                 args:
                         since
                                 - date of the earliest returned value */

            IRestResponse response = await ExecuteCommand("deleted?since=" + date);

            return response.Content;
        }


        public async Task<JObject> GetComicInfo(int id)
        {
            /*     /comic/{id}
                     - info about specific comic
                     */

            IRestResponse response = await ExecuteCommand("/comic/" + id);

            JObject o = JObject.Parse(response.Content);

            return o;
        }   

        public async Task<byte[]> GetComicPage(int id, int pagenum = 0)
        {
            //TODO: Add height arg.
            /*/comic/{id}/page/{pagenum}
                     - return specific page image of specific comic
                         args:
                             max_height
                                 - will resize image*/
            IRestResponse response = await ExecuteCommand("/comic/" + id + "/page/" + pagenum);

            byte[] imageBytes = response.RawBytes;
 
            return imageBytes;
        }
                 
        public async void SetBookmark(int id, string pagenum = "clear")
        {
            /*/comic/{id}/page/{pagenum}/bookmark
                     - sets the time of last access and last page read for the comic.
                         client would fetch this for each page turn
                         if {pagenum} is "clear"  clears bookmark for the given book*/
            IRestResponse response = await ExecuteCommand("/comic/" + id + "/page/" + pagenum + "/bookmark");

        }

        public async Task<byte[]> GetThumbnail(int id)
        {
            /* /comic/{id}/thumbnail
                     - return specific small cover image of specific comic*/
            IRestResponse response = await ExecuteCommand("/comic/" + id  + "/thumbnail");

            byte[] imageBytes = response.RawBytes;

            return imageBytes;
        }

        public async Task<byte[]> GetComicFile(int id)
        {
            /* /comic/{id}/file
                     - return entire specific comic file*/
            IRestResponse response = await ExecuteCommand("/comic/" + id + "/file");

            byte[] imageBytes = response.RawBytes;

            return imageBytes;
        }

        /*/comiclist
           - return list of comics info.  with no args, returns entire list
                   args:
                       series
                           filter by series (wildcard accepted)
                       title
                           filter by issue title (wildcard accepted)
                       path
                           filter by file path (wildcard accepted)
                       character
                           filter by character (wildcard accepted)
                       team
                           filter by team (wildcard accepted)
                       location
                           filter by location (wildcard accepted)
                       storyarc
                           filter by story arc (wildcard accepted)
                       genre
                           filter by genre (wildcard accepted)
                       tag
                           filter by generic tag (wildcard accepted)
                       volume
                           filter by volume (wildcard accepted)
                       publisher
                           filter by publisher (wildcard accepted)
                       credit
                           filter by creator credit (wildcard accepted)
                           should be a name optionally followed by a ":" and the role. e.g:
                               credit=Brian*Vaughn
                               credit=*Buscema:inker
                       start_date
                           the beginning publication date of the resultset
                       end_date
                           the end publication date of the resultset
                       added_since
                           only comics added the to database since given date
                       lastread_since
                           only comics that have been read since given date
                           (Set this value to something like "1970" to filter out unread comics)
                       per_page
                           max amount of results to be returned
                       offset
                           the starting offset of the query resultset
                       order
                           order by given key.  if key has "-" prepended, order descending
                           available sort keys:
                               series, title, volume, issue, publisher, path, modified, added, lastread, date

                   wildcard character is * (asterisk)

                   date format is "YYYY-MM-DD hh:mm:ss", where the right-most (most granular) portions may be omitted
/*/
        public async Task<JObject> GetComicList(
            string series = "", 
            string title = "",
            string path = "",
            string character = "",
            string team = "",
            string location = "",
            string storyarc = "",
            string genre = "",
            string tag = "",
            string volume = "",
            string publisher = "",
            string credit = "",
            string start_date = "",
            string end_date = "",
            string added_since = "",
            string lastread_Since = "",
            string per_page = "",
            string offset = "",
            string order = "")
        {
          
            IRestResponse response = await ExecuteCommand("/comiclist?series=" + series + 
                "&title=" + title + 
                "&path=" + path + 
                "&character=" + character + 
                "&team=" + team + 
                "&location=" + location +
                "&storyarc=" + storyarc + 
                "&genre=" + genre +
                "&tag=" + tag +
                "&volume=" + volume +
                "&publisher=" + publisher +
                "&credit=" + credit +
                "&start_date=" + start_date +
                "&end_date=" + end_date +
                "&added_since=" + added_since +
                "&lastread_Since=" + lastread_Since +
                "&per_page=" + per_page +
                "&offset=" + offset +
                "&order=" + order);

            JObject o = JObject.Parse(response.Content);

            return o;
        }
                 
        public async Task<JObject> GetFolders(string path = "")
        {
            /*/folders/[path]
                     - Return list of folders  with names and access URLS), and list of comics in the specific folder
                       Without a path, returns just the top level folders
                       */
            IRestResponse response = await ExecuteCommand("/folders/" + path);

            JObject o = JObject.Parse(response.Content);

            return o;
        }


        readonly string msBaseUrl;
    }
}
