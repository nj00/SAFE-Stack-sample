module Shared.BlogModels

open System

/// <summary>
/// TaxonomyTypeの列挙型
/// </summary>
type TaxonomyTypeEnum =
    | Category = 0
    | Tag = 1
    | Series = 2


/// <summary>
/// 投稿記事の分類情報を表します。
/// </summary>
type Taxonomy = {
    /// <summary>
    /// id
    /// </summary>
    Id : int64;

    /// <summary>
    /// 分類タイプ
    /// </summary>
    Type : TaxonomyTypeEnum;

    /// <summary>
    /// 名称
    /// </summary>
    Name : string;

    /// <summary>
    /// カテゴリのアドレスを定義するために使用されるurlスラッグ。
    /// </summary>
    UrlSlug : string;

    /// <summary>
    /// 説明
    /// </summary>
    Description : string option;
}


